using Users.Domain;
using BuildingBlocks.Abstractions;

namespace Users.Application;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IEnumerable<UserModel>>> GetUsersAsync()
    {
        return Result<IEnumerable<UserModel>>.Success(await _repo.ListAsync());
    }

    public async Task<Result<UserModel>> GetUserAsync(Guid id)
    {
        var user = await _repo.GetAsync(id);
        return user is null ? Result<UserModel>.Failure("Not found") : Result<UserModel>.Success(user);
    }

    public async Task<Result<UserModel>> CreateUserAsync(UserModel user)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
            return Result<UserModel>.Failure("Email is required");

        var created = await _repo.CreateAsync(user);
        return Result<UserModel>.Success(created);
    }

    public async Task<Result<UserModel>> UpdateUserAsync(Guid id, UserModel user)
    {
        var updated = await _repo.UpdateAsync(id, user);
        return updated is null ? Result<UserModel>.Failure("Not found") : Result<UserModel>.Success(updated);
    }

    public async Task<Result<bool>> DeleteUserAsync(Guid id)
    {
        var deleted = await _repo.DeleteAsync(id);
        return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Not found");
    }
}
