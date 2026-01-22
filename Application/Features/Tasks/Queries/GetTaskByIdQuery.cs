using MediatR;
using TodoApi.Application.Abstractions;
using TodoApi.Domain.Common;
using TodoApi.Models;
using TodoApi.Presentation.Common;
using TodoApi.Presentation.DTOs;
using Microsoft.Extensions.Logging;

namespace TodoApi.Application.Features.Tasks.Queries;

/// <summary>
/// Query to retrieve a task by ID
/// </summary>
public sealed record GetTaskByIdQuery(
    int TaskId,
    int RequestedById
) : IRequest<Result<TaskDto>>;

/// <summary>
/// Handler for GetTaskByIdQuery
/// </summary>
public sealed class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectAccessService _projectAccess;
    private readonly ILogger<GetTaskByIdQueryHandler> _logger;

    public GetTaskByIdQueryHandler(
        ITaskRepository taskRepository,
        IProjectAccessService projectAccess,
        ILogger<GetTaskByIdQueryHandler> logger)
    {
        _taskRepository = taskRepository;
        _projectAccess = projectAccess;
        _logger = logger;
    }

    public async Task<Result<TaskDto>> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var task = await _taskRepository.GetByIdWithDetailsAsync(query.TaskId);
            if (task == null)
                return Result<TaskDto>.Failure("Task not found");

            var hasAccess = await _projectAccess.UserHasProjectAccessAsync(task.ProjectId, query.RequestedById);
            if (!hasAccess)
                return Result<TaskDto>.Failure("You don't have access to this task");

            return Result<TaskDto>.Success(MapToTaskDto(task));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task {TaskId}", query.TaskId);
            return Result<TaskDto>.Failure($"Error retrieving task: {ex.Message}");
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
