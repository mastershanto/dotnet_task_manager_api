using MediatR;
using TodoApi.Application.Abstractions;
using TodoApi.Domain.Common;
using TodoApi.Models;
using TodoApi.Presentation.Common;
using TodoApi.Presentation.DTOs;
using Microsoft.Extensions.Logging;

namespace TodoApi.Application.Features.Tasks.Commands;

/// <summary>
/// Command to create a new task
/// </summary>
public sealed record CreateTaskCommand(
    string Title,
    string? Description,
    int ProjectId,
    int? AssigneeId,
    int Priority,
    DateTime? DueDate,
    DateTime? StartDate,
    int? EstimatedHours,
    string? Tags,
    int CreatedById
) : IRequest<Result<TaskDto>>;

/// <summary>
/// Handler for CreateTaskCommand
/// </summary>
public sealed class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectAccessService _projectAccess;
    private readonly ILogger<CreateTaskCommandHandler> _logger;

    public CreateTaskCommandHandler(
        ITaskRepository taskRepository,
        IProjectAccessService projectAccess,
        ILogger<CreateTaskCommandHandler> logger)
    {
        _taskRepository = taskRepository;
        _projectAccess = projectAccess;
        _logger = logger;
    }

    public async Task<Result<TaskDto>> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        try
        {
            // Verify project exists and user has access
            var hasAccess = await _projectAccess.UserHasProjectAccessAsync(command.ProjectId, command.CreatedById);
            if (!hasAccess)
                return Result<TaskDto>.Failure("You don't have permission to create tasks in this project");

            // Create task using model
            var task = new TaskItem
            {
                Title = command.Title,
                Description = command.Description,
                ProjectId = command.ProjectId,
                AssigneeId = command.AssigneeId,
                CreatedById = command.CreatedById,
                Priority = (TaskPriority)command.Priority,
                Status = Models.TaskStatus.Todo,
                CreatedAt = DateTime.UtcNow
            };

            // Set optional properties
            if (command.DueDate.HasValue)
                task.DueDate = command.DueDate.Value;

            if (command.StartDate.HasValue)
                task.StartDate = command.StartDate.Value;

            if (command.EstimatedHours.HasValue)
                task.EstimatedHours = command.EstimatedHours.Value;

            if (!string.IsNullOrEmpty(command.Tags))
                task.Tags = command.Tags;

            // Add to repository
            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            _logger.LogInformation("Task {TaskId} created successfully by user {UserId}", task.Id, command.CreatedById);

            return Result<TaskDto>.Success(MapToTaskDto(task));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return Result<TaskDto>.Failure($"Error creating task: {ex.Message}");
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
            DueDate = task.DueDate,
            StartDate = task.StartDate,
            EstimatedHours = task.EstimatedHours,
            Tags = task.Tags,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}
