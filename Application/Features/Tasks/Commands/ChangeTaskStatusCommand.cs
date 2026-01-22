using MediatR;
using TodoApi.Application.Abstractions;
using TodoApi.Domain.Common;
using TodoApi.Models;
using TodoApi.Presentation.Common;
using TodoApi.Presentation.DTOs;
using Microsoft.Extensions.Logging;

namespace TodoApi.Application.Features.Tasks.Commands;

/// <summary>
/// Command to change task status
/// </summary>
public sealed record ChangeTaskStatusCommand(
    int TaskId,
    int NewStatus,
    int ChangedById
) : IRequest<Result<TaskDto>>;

/// <summary>
/// Handler for ChangeTaskStatusCommand
/// </summary>
public sealed class ChangeTaskStatusCommandHandler : IRequestHandler<ChangeTaskStatusCommand, Result<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectAccessService _projectAccess;
    private readonly ILogger<ChangeTaskStatusCommandHandler> _logger;

    public ChangeTaskStatusCommandHandler(
        ITaskRepository taskRepository,
        IProjectAccessService projectAccess,
        ILogger<ChangeTaskStatusCommandHandler> logger)
    {
        _taskRepository = taskRepository;
        _projectAccess = projectAccess;
        _logger = logger;
    }

    public async Task<Result<TaskDto>> Handle(ChangeTaskStatusCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var task = await _taskRepository.GetByIdAsync(command.TaskId);
            if (task == null)
                return Result<TaskDto>.Failure("Task not found");

            // Verify authorization
            var hasAccess = await _projectAccess.UserHasProjectAccessAsync(task.ProjectId, command.ChangedById);
            if (!hasAccess)
                return Result<TaskDto>.Failure("You don't have permission to change this task's status");

            // Change status
            task.Status = (Models.TaskStatus)command.NewStatus;
            
            // Auto-set completion date if completing
            if (command.NewStatus == (int)Models.TaskStatus.Done)
                task.CompletedAt = DateTime.UtcNow;
            else if (command.NewStatus != (int)Models.TaskStatus.Done)
                task.CompletedAt = null;

            task.UpdatedAt = DateTime.UtcNow;
            await _taskRepository.SaveChangesAsync();

            _logger.LogInformation("Task {TaskId} status changed to {Status} by user {UserId}", 
                command.TaskId, command.NewStatus, command.ChangedById);

            return Result<TaskDto>.Success(MapToTaskDto(task));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing task status for task {TaskId}", command.TaskId);
            return Result<TaskDto>.Failure($"Error changing task status: {ex.Message}");
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
