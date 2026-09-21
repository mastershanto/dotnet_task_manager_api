using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Tasks.Domain;

namespace Tasks.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler : ICommandHandler<DeleteTaskCommand>
{
    private readonly ITaskRepository _repository;

    public DeleteTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _repository.DeleteAsync(request.Id, cancellationToken);
        return deleted 
            ? Result<bool>.Success(true) 
            : Result<bool>.Failure($"Task with ID '{request.Id}' was not found.");
    }
}
