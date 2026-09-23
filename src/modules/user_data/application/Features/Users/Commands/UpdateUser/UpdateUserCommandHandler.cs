using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Users.Domain;

namespace Users.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UserModel>
{
    private readonly IUserRepository _repository;

    public UpdateUserCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UserModel>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new UserModel
        {
            Id = request.Id,
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLowerInvariant()
        };

        var updated = await _repository.UpdateAsync(request.Id, user);
        return updated is not null
            ? Result<UserModel>.Success(updated)
            : Result<UserModel>.Failure($"User with ID '{request.Id}' was not found.");
    }
}
