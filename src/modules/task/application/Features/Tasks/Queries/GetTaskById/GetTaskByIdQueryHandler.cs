using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Tasks.Domain;

namespace Tasks.Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IQueryHandler<GetTaskByIdQuery, TaskItemModel>
{
    private readonly ITaskRepository _repository;

    public GetTaskByIdQueryHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TaskItemModel>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetAsync(request.Id, cancellationToken);
        return task is null 
            ? Result<TaskItemModel>.Failure($"Task with ID '{request.Id}' was not found.") 
            : Result<TaskItemModel>.Success(task);
    }
}
