using BuildingBlocks.CQRS;
using Tasks.Domain;

namespace Tasks.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// নতুন টাস্ক তৈরির CQRS Command (Write Request):
/// </summary>
public record CreateTaskCommand(
    string Title,
    string Description,
    TaskItemStatus Status = TaskItemStatus.Todo,
    TaskPriority Priority = TaskPriority.Medium,
    DateTime? DueDate = null,
    Guid? CategoryId = null,
    Guid? AssignedUserId = null
) : ICommand<TaskItemModel>;
