using Users.Application.Features.Users.Commands.CreateUser;
using Users.Application.Features.Users.Commands.DeleteUser;
using Users.Application.Features.Users.Commands.UpdateUser;
using Users.Application.Features.Users.Queries.GetUserById;
using Users.Application.Features.Users.Queries.GetUsers;
using Users.Domain;
using Xunit;

namespace Api.Tests;

public class FakeUserRepository : IUserRepository
{
    private readonly List<UserModel> _users = new();

    public Task<IEnumerable<UserModel>> ListAsync() =>
        Task.FromResult<IEnumerable<UserModel>>(_users.ToList());

    public Task<UserModel?> GetAsync(Guid id) =>
        Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

    public Task<UserModel> CreateAsync(UserModel user)
    {
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<UserModel?> UpdateAsync(Guid id, UserModel user)
    {
        var idx = _users.FindIndex(u => u.Id == id);
        if (idx == -1) return Task.FromResult<UserModel?>(null);
        _users[idx] = user;
        return Task.FromResult<UserModel?>(user);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var item = _users.FirstOrDefault(u => u.Id == id);
        if (item is null) return Task.FromResult(false);
        _users.Remove(item);
        return Task.FromResult(true);
    }
}

public class UserCqrsTests
{
    [Fact]
    public async Task CreateUserCommand_Success_WhenValid()
    {
        var repo = new FakeUserRepository();
        var handler = new CreateUserCommandHandler(repo);

        var command = new CreateUserCommand("Dancer Jane", "jane@example.com");
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Dancer Jane", result.Value.Name);
        Assert.Equal("jane@example.com", result.Value.Email);
    }

    [Fact]
    public void CreateUserValidator_Fails_WhenEmailIsInvalid()
    {
        var validator = new CreateUserCommandValidator();
        var command = new CreateUserCommand("Valid Name", "not-an-email");

        var validationResult = validator.Validate(command);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(CreateUserCommand.Email));
    }

    [Fact]
    public async Task GetUsersQuery_ReturnsAll()
    {
        var repo = new FakeUserRepository();
        await repo.CreateAsync(new UserModel { Name = "User 1", Email = "u1@test.com" });
        await repo.CreateAsync(new UserModel { Name = "User 2", Email = "u2@test.com" });

        var handler = new GetUsersQueryHandler(repo);
        var result = await handler.Handle(new GetUsersQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task UpdateUserCommand_ReturnsFailure_WhenNotFound()
    {
        var repo = new FakeUserRepository();
        var handler = new UpdateUserCommandHandler(repo);

        var command = new UpdateUserCommand(Guid.NewGuid(), "New Name", "new@test.com");
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Errors.First(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteUserCommand_ReturnsSuccess_WhenExists()
    {
        var repo = new FakeUserRepository();
        var existing = await repo.CreateAsync(new UserModel { Name = "To Delete", Email = "del@test.com" });
        var handler = new DeleteUserCommandHandler(repo);

        var result = await handler.Handle(new DeleteUserCommand(existing.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }
}
