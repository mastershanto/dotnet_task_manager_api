using BuildingBlocks.CQRS;
using Tasks.Domain;

namespace Tasks.Application.Features.Tasks.Queries.GetTasks;

/// <summary>
/// সমস্ত টাস্ক পাওয়ার CQRS Query (Read Request):
/// অপশনাল ফিল্টারিং (Status, CategoryId) সাপোর্ট করে।
/// </summary>
public record GetTasksQuery(
    TaskItemStatus? Status = null,
    Guid? CategoryId = null
) : IQuery<IEnumerable<TaskItemModel>>;
