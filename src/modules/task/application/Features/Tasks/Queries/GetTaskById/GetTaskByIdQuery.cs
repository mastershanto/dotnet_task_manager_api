using BuildingBlocks.CQRS;
using Tasks.Domain;

namespace Tasks.Application.Features.Tasks.Queries.GetTaskById;

/// <summary>
/// নির্দিষ্ট আইডির টাস্ক রিড করার CQRS Query:
/// </summary>
public record GetTaskByIdQuery(Guid Id) : IQuery<TaskItemModel>;
