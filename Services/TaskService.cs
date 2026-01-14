using TodoApi.DTOs;
using TodoApi.Models;
using TodoApi.Application.Abstractions;

namespace TodoApi.Services;

public interface ITaskService
{
    Task<TaskDto> GetTaskByIdAsync(int taskId, int userId);
    Task<PaginatedResponse<TaskDto>> GetTasksAsync(int projectId, int userId, QueryParams queryParams);
    Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, int createdById);
    Task<TaskDto> UpdateTaskAsync(int taskId, UpdateTaskDto dto, int userId);
    Task DeleteTaskAsync(int taskId, int userId);
    Task<List<TaskDto>> GetUserTasksAsync(int userId, QueryParams queryParams);
    Task ChangeTaskStatusAsync(int taskId, int newStatus, int userId);
    Task AssignTaskAsync(int taskId, int? assigneeId, int userId);
}

public class TaskService : ITaskService
{
    private readonly ITaskRepository _tasks;
    private readonly IProjectAccessService _projectAccess;
    private readonly ITaskHistoryRepository _history;
    private readonly ILogger<TaskService> _logger;

    public TaskService(
        ITaskRepository tasks,
        IProjectAccessService projectAccess,
        ITaskHistoryRepository history,
        ILogger<TaskService> logger)
    {
        _tasks = tasks;
        _projectAccess = projectAccess;
        _history = history;
        _logger = logger;
    }

    public async Task<TaskDto> GetTaskByIdAsync(int taskId, int userId)
    {
        var task = await _tasks.GetByIdWithDetailsAsync(taskId);

        if (task == null)
            throw new KeyNotFoundException($"Task {taskId} not found");

        // Check if user has access
        var hasAccess = await _projectAccess.UserHasProjectAccessAsync(task.ProjectId, userId);
        if (!hasAccess)
            throw new UnauthorizedAccessException("You don't have access to this task");

        return MapToTaskDto(task);
    }

    public async Task<PaginatedResponse<TaskDto>> GetTasksAsync(int projectId, int userId, QueryParams queryParams)
    {
        // Verify user has access to project
        var hasAccess = await _projectAccess.UserHasProjectAccessAsync(projectId, userId);
        if (!hasAccess)
            throw new UnauthorizedAccessException("You don't have access to this project");

        var (tasks, totalCount) = await _tasks.GetProjectTasksAsync(projectId, queryParams);

        return new PaginatedResponse<TaskDto>
        {
            Items = tasks.Select(MapToTaskDto).ToList(),
            TotalCount = totalCount,
            PageNumber = queryParams.PageNumber,
            PageSize = queryParams.PageSize
        };
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, int createdById)
    {
        // Verify project exists and user has access
        var projectExists = await _projectAccess.ProjectExistsAsync(dto.ProjectId);
        if (!projectExists)
            throw new KeyNotFoundException("Project not found");

        var hasAccess = await _projectAccess.UserHasProjectAccessAsync(dto.ProjectId, createdById);
        if (!hasAccess)
            throw new UnauthorizedAccessException("You don't have permission to create tasks in this project");

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            ProjectId = dto.ProjectId,
            AssigneeId = dto.AssigneeId,
            CreatedById = createdById,
            Priority = (TaskPriority)dto.Priority,
            DueDate = dto.DueDate,
            StartDate = dto.StartDate,
            EstimatedHours = dto.EstimatedHours,
            Tags = dto.Tags,
            ParentTaskId = dto.ParentTaskId
        };

        await _tasks.AddAsync(task);
        await _tasks.SaveChangesAsync();

        // Record in history
        await RecordTaskHistoryAsync(task.Id, createdById, "Status", null, task.Status.ToString());

        _logger.LogInformation("Task {TaskId} created by user {UserId}", task.Id, createdById);

        return await GetTaskByIdAsync(task.Id, createdById);
    }

    public async Task<TaskDto> UpdateTaskAsync(int taskId, UpdateTaskDto dto, int userId)
    {
        var task = await _tasks.GetByIdAsync(taskId);
        if (task == null)
            throw new KeyNotFoundException("Task not found");

        var hasAccess = await _projectAccess.UserHasProjectAccessAsync(task.ProjectId, userId);
        if (!hasAccess)
            throw new UnauthorizedAccessException("You don't have permission to update this task");

        // Record changes
        if (!string.IsNullOrEmpty(dto.Title) && task.Title != dto.Title)
        {
            await RecordTaskHistoryAsync(taskId, userId, "Title", task.Title, dto.Title);
            task.Title = dto.Title;
        }

        if (!string.IsNullOrEmpty(dto.Description) && task.Description != dto.Description)
        {
            await RecordTaskHistoryAsync(taskId, userId, "Description", task.Description, dto.Description);
            task.Description = dto.Description;
        }

        if (dto.AssigneeId.HasValue && task.AssigneeId != dto.AssigneeId)
        {
            await RecordTaskHistoryAsync(taskId, userId, "Assignee", 
                task.AssigneeId?.ToString(), dto.AssigneeId.ToString());
            task.AssigneeId = dto.AssigneeId;
        }

        if (dto.Status.HasValue && (int)task.Status != dto.Status)
        {
            await RecordTaskHistoryAsync(taskId, userId, "Status", task.Status.ToString(), 
                ((Models.TaskStatus)dto.Status).ToString());
            task.Status = (Models.TaskStatus)dto.Status;
        }

        if (dto.Priority.HasValue && (int)task.Priority != dto.Priority)
        {
            await RecordTaskHistoryAsync(taskId, userId, "Priority", 
                task.Priority.ToString(), ((TaskPriority)dto.Priority).ToString());
            task.Priority = (TaskPriority)dto.Priority;
        }

        if (dto.Progress.HasValue)
            task.Progress = dto.Progress;

        if (dto.DueDate.HasValue)
            task.DueDate = dto.DueDate;

        if (dto.EstimatedHours.HasValue)
            task.EstimatedHours = dto.EstimatedHours;

        if (dto.ActualHours.HasValue)
            task.ActualHours = dto.ActualHours;

        if (!string.IsNullOrEmpty(dto.Tags))
            task.Tags = dto.Tags;

        if (dto.IsBlocked.HasValue)
            task.IsBlocked = dto.IsBlocked.Value;

        if (!string.IsNullOrEmpty(dto.BlockedReason))
            task.BlockedReason = dto.BlockedReason;

        task.UpdatedAt = DateTime.UtcNow;

        await _tasks.SaveChangesAsync();
        _logger.LogInformation("Task {TaskId} updated by user {UserId}", taskId, userId);

        return await GetTaskByIdAsync(taskId, userId);
    }

    public async Task DeleteTaskAsync(int taskId, int userId)
    {
        var task = await _tasks.GetByIdAsync(taskId);
        if (task == null)
            throw new KeyNotFoundException("Task not found");

        var hasAccess = await _projectAccess.UserHasProjectAccessAsync(task.ProjectId, userId);
        if (!hasAccess)
            throw new UnauthorizedAccessException("You don't have permission to delete this task");

        task.DeletedAt = DateTime.UtcNow;
        await _tasks.SaveChangesAsync();

        _logger.LogInformation("Task {TaskId} deleted by user {UserId}", taskId, userId);
    }

    public async Task<List<TaskDto>> GetUserTasksAsync(int userId, QueryParams queryParams)
    {
        var tasks = await _tasks.GetUserTasksAsync(userId, queryParams);

        return tasks.Select(MapToTaskDto).ToList();
    }

    public async Task ChangeTaskStatusAsync(int taskId, int newStatus, int userId)
    {
        var task = await _tasks.GetByIdAsync(taskId);
        if (task == null)
            throw new KeyNotFoundException("Task not found");

        var hasAccess = await _projectAccess.UserHasProjectAccessAsync(task.ProjectId, userId);
        if (!hasAccess)
            throw new UnauthorizedAccessException("You don't have permission to update this task");

        var oldStatus = task.Status.ToString();
        task.Status = (Models.TaskStatus)newStatus;
        
        if (newStatus == (int)Models.TaskStatus.Done)
            task.CompletedAt = DateTime.UtcNow;

        await RecordTaskHistoryAsync(taskId, userId, "Status", oldStatus, task.Status.ToString());
        await _tasks.SaveChangesAsync();
    }

    public async Task AssignTaskAsync(int taskId, int? assigneeId, int userId)
    {
        var task = await _tasks.GetByIdAsync(taskId);
        if (task == null)
            throw new KeyNotFoundException("Task not found");

        var hasAccess = await _projectAccess.UserHasProjectAccessAsync(task.ProjectId, userId);
        if (!hasAccess)
            throw new UnauthorizedAccessException("You don't have permission to update this task");

        var oldAssignee = task.AssigneeId?.ToString() ?? "Unassigned";
        var newAssignee = assigneeId?.ToString() ?? "Unassigned";

        task.AssigneeId = assigneeId;
        await RecordTaskHistoryAsync(taskId, userId, "Assignee", oldAssignee, newAssignee);
        await _tasks.SaveChangesAsync();
    }

    private async Task RecordTaskHistoryAsync(int taskId, int userId, string fieldName, string? oldValue, string? newValue)
    {
        var history = new TaskHistory
        {
            TaskId = taskId,
            ChangedById = userId,
            FieldName = fieldName,
            OldValue = oldValue,
            NewValue = newValue
        };

        await _history.AddAsync(history);
        await _history.SaveChangesAsync();
    }

    private static TaskDto MapToTaskDto(TaskItem task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            ProjectId = task.ProjectId,
            AssigneeId = task.AssigneeId,
            AssigneeName = task.Assignee?.FullName ?? task.Assignee?.Username,
            CreatedById = task.CreatedById,
            CreatedByName = task.CreatedBy?.FullName ?? task.CreatedBy?.Username,
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
            UpdatedAt = task.UpdatedAt,
            CommentCount = task.Comments?.Count ?? 0,
            AttachmentCount = task.Attachments?.Count ?? 0
        };
    }
}
