using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Users.Domain;

namespace Users.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IUserRepository _repository;

    public DeleteUserCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _repository.DeleteAsync(request.Id);
        return deleted
            ? Result<bool>.Success(true)
            : Result<bool>.Failure($"User with ID '{request.Id}' was not found.");
    }
}
