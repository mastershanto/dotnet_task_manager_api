using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Users.Domain;

namespace Users.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserModel>
{
    private readonly IUserRepository _repository;

    public CreateUserCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UserModel>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(user);
        return Result<UserModel>.Success(created);
    }
}
