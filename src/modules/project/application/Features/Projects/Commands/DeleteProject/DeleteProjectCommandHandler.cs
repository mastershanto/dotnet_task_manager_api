using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Projects.Domain;

namespace Projects.Application.Features.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandler : ICommandHandler<DeleteProjectCommand>
{
    private readonly IProjectRepository _repository;

    public DeleteProjectCommandHandler(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _repository.DeleteAsync(request.Id, cancellationToken);
        return deleted
            ? Result<bool>.Success(true)
            : Result<bool>.Failure($"Project with ID '{request.Id}' was not found.");
    }
}
