using MediatR;
using TodoApi.Application.Abstractions;
using TodoApi.Domain.Common;
using TodoApi.Models;
using TodoApi.Presentation.Common;
using TodoApi.Presentation.DTOs;
using Microsoft.Extensions.Logging;

namespace TodoApi.Application.Features.Tasks.Commands;

/// <summary>
/// Command to assign a task to a user
/// </summary>
public sealed record AssignTaskCommand(
    int TaskId,
    int? NewAssigneeId,
    int AssignedById
) : IRequest<Result<TaskDto>>;

/// <summary>
/// Handler for AssignTaskCommand
/// </summary>
public sealed class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, Result<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectAccessService _projectAccess;
    private readonly ILogger<AssignTaskCommandHandler> _logger;

    public AssignTaskCommandHandler(
        ITaskRepository taskRepository,
        IProjectAccessService projectAccess,
        ILogger<AssignTaskCommandHandler> logger)
    {
        _taskRepository = taskRepository;
        _projectAccess = projectAccess;
        _logger = logger;
    }

    public async Task<Result<TaskDto>> Handle(AssignTaskCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var task = await _taskRepository.GetByIdAsync(command.TaskId);
            if (task == null)
                return Result<TaskDto>.Failure("Task not found");

            // Verify authorization
            var hasAccess = await _projectAccess.UserHasProjectAccessAsync(task.ProjectId, command.AssignedById);
            if (!hasAccess)
                return Result<TaskDto>.Failure("You don't have permission to assign this task");

            // Assign or unassign
            if (command.NewAssigneeId.HasValue)
            {
                task.AssigneeId = command.NewAssigneeId.Value;
                _logger.LogInformation("Task {TaskId} assigned to user {AssigneeId} by user {UserId}", 
                    command.TaskId, command.NewAssigneeId.Value, command.AssignedById);
            }
            else
            {
                task.AssigneeId = null;
                _logger.LogInformation("Task {TaskId} unassigned by user {UserId}", 
                    command.TaskId, command.AssignedById);
            }

            task.UpdatedAt = DateTime.UtcNow;
            await _taskRepository.SaveChangesAsync();

            return Result<TaskDto>.Success(MapToTaskDto(task));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning task {TaskId}", command.TaskId);
            return Result<TaskDto>.Failure($"Error assigning task: {ex.Message}");
        }
    }

    private TaskDto MapToTaskDto(TaskItem task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            ProjectId = task.ProjectId,
            AssigneeId = task.AssigneeId,
            Status = (int)task.Status,
            StatusName = task.Status.ToString(),
            Priority = (int)task.Priority,
            PriorityName = task.Priority.ToString(),
            Progress = task.Progress,
            DueDate = task.DueDate,
            StartDate = task.StartDate,
            CompletedAt = task.CompletedAt,
            EstimatedHours = task.EstimatedHours,
            ActualHours = task.ActualHours,
            Tags = task.Tags,
            IsBlocked = task.IsBlocked,
            BlockedReason = task.BlockedReason,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}
