using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Tasks.Domain;

namespace Tasks.Application.Features.Tasks.Queries.GetTasks;

/// <summary>
/// GetTasksQuery হ্যান্ডলার (Optimized Read side):
/// </summary>
public class GetTasksQueryHandler : IQueryHandler<GetTasksQuery, IEnumerable<TaskItemModel>>
{
    private readonly ITaskRepository _repository;

    public GetTasksQueryHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<TaskItemModel>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _repository.ListAsync(cancellationToken);

        if (request.Status.HasValue)
        {
            tasks = tasks.Where(t => t.Status == request.Status.Value);
        }

        if (request.CategoryId.HasValue)
        {
            tasks = tasks.Where(t => t.CategoryId == request.CategoryId.Value);
        }

        return Result<IEnumerable<TaskItemModel>>.Success(tasks);
    }
}
