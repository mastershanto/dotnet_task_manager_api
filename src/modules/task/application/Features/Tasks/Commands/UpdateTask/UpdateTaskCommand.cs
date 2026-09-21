using BuildingBlocks.CQRS;
using Tasks.Domain;

namespace Tasks.Application.Features.Tasks.Commands.UpdateTask;

/// <summary>
/// বিদ্যমান টাস্ক আপডেট করার CQRS Command:
/// </summary>
public record UpdateTaskCommand(
    Guid Id,
    string Title,
    string Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid? CategoryId,
    Guid? AssignedUserId
) : ICommand<TaskItemModel>;
