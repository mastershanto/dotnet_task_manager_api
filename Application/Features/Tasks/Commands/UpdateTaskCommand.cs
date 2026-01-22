using MediatR;
using TodoApi.Application.Abstractions;
using TodoApi.Domain.Common;
using TodoApi.Models;
using TodoApi.Presentation.Common;
using TodoApi.Presentation.DTOs;
using Microsoft.Extensions.Logging;

namespace TodoApi.Application.Features.Tasks.Commands;

/// <summary>
/// Command to update an existing task
/// </summary>
public sealed record UpdateTaskCommand(
    int TaskId,
    string? Title,
    string? Description,
    int? AssigneeId,
    int? Status,
    int? Priority,
    double? Progress,
    DateTime? DueDate,
    int? EstimatedHours,
    int? ActualHours,
    string? Tags,
    bool? IsBlocked,
    string? BlockedReason,
    DateTime? StartDate,
    int UpdatedById
) : IRequest<Result<TaskDto>>;

/// <summary>
/// Handler for UpdateTaskCommand
/// </summary>
public sealed class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectAccessService _projectAccess;
    private readonly ILogger<UpdateTaskCommandHandler> _logger;

    public UpdateTaskCommandHandler(
        ITaskRepository taskRepository,
        IProjectAccessService projectAccess,
        ILogger<UpdateTaskCommandHandler> logger)
    {
        _taskRepository = taskRepository;
        _projectAccess = projectAccess;
        _logger = logger;
    }

    public async Task<Result<TaskDto>> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var task = await _taskRepository.GetByIdAsync(command.TaskId);
            if (task == null)
                return Result<TaskDto>.Failure("Task not found");

            // Verify user has access to the project
            var hasAccess = await _projectAccess.UserHasProjectAccessAsync(task.ProjectId, command.UpdatedById);
            if (!hasAccess)
                return Result<TaskDto>.Failure("You don't have permission to update this task");

            // Apply updates to model properties
            if (!string.IsNullOrEmpty(command.Title))
                task.Title = command.Title;

            if (command.Description != null)
                task.Description = command.Description;

            if (command.Priority.HasValue)
                task.Priority = (Models.TaskPriority)command.Priority.Value;

            if (command.Status.HasValue)
                task.Status = (Models.TaskStatus)command.Status.Value;

            if (command.DueDate.HasValue)
                task.DueDate = command.DueDate.Value;

            if (command.StartDate.HasValue)
                task.StartDate = command.StartDate.Value;

            if (command.EstimatedHours.HasValue)
                task.EstimatedHours = command.EstimatedHours.Value;

            if (command.ActualHours.HasValue)
                task.ActualHours = command.ActualHours.Value;

            if (command.Progress.HasValue)
                task.Progress = command.Progress.Value;

            if (!string.IsNullOrEmpty(command.Tags))
                task.Tags = command.Tags;

            if (command.IsBlocked.HasValue)
            {
                task.IsBlocked = command.IsBlocked.Value;
                if (command.IsBlocked.Value && !string.IsNullOrEmpty(command.BlockedReason))
                    task.BlockedReason = command.BlockedReason;
                else if (!command.IsBlocked.Value)
                    task.BlockedReason = null;
            }

            if (command.AssigneeId.HasValue)
                task.AssigneeId = command.AssigneeId.Value;

            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.SaveChangesAsync();

            _logger.LogInformation("Task {TaskId} updated successfully by user {UserId}", command.TaskId, command.UpdatedById);

            return Result<TaskDto>.Success(MapToTaskDto(task));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task {TaskId}", command.TaskId);
            return Result<TaskDto>.Failure($"Error updating task: {ex.Message}");
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
