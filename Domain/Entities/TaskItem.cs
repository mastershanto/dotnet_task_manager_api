using TodoApi.Domain.Common;
using TodoApi.Domain.Events;
using TodoApi.Domain.Exceptions;

namespace TodoApi.Domain.Entities;

/// <summary>
/// Rich Domain Entity with business logic - Enterprise DDD pattern
/// </summary>
public class TaskItem : AuditableEntity
{
    // Private setters enforce business rules through methods
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public int ProjectId { get; private set; }
    public int? AssigneeId { get; private set; }
    public TaskStatus Status { get; private set; } = TaskStatus.Todo;
    public TaskPriority Priority { get; private set; } = TaskPriority.Medium;
    public double? Progress { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int? EstimatedHours { get; private set; }
    public int? ActualHours { get; private set; }
    public string? Tags { get; private set; }
    public int OrderIndex { get; private set; }
    public bool IsBlocked { get; private set; }
    public string? BlockedReason { get; private set; }
    public int? ParentTaskId { get; private set; }

    // Navigation properties
    public TaskItem? ParentTask { get; private set; }
    public ICollection<TaskItem> Subtasks { get; private set; } = new List<TaskItem>();

    // Private constructor for EF Core
    private TaskItem() { }

    /// <summary>
    /// Factory method to create a new task - enforces business rules
    /// </summary>
    public static TaskItem Create(
        string title,
        int projectId,
        int createdById,
        string? description = null,
        TaskPriority priority = TaskPriority.Medium)
    {
        ValidateTitle(title);

        var task = new TaskItem
        {
            Title = title.Trim(),
            Description = description?.Trim(),
            ProjectId = projectId,
            Priority = priority,
            Status = TaskStatus.Todo,
            OrderIndex = 0
        };

        task.MarkAsCreated(createdById);

        // Raise domain event
        task.AddDomainEvent(new TaskCreatedEvent
        {
            TaskId = task.Id,
            ProjectId = projectId,
            CreatedById = createdById,
            Title = title
        });

        return task;
    }

    /// <summary>
    /// Business rule: Update task details
    /// </summary>
    public void Update(string title, string? description, int updatedById)
    {
        ValidateTitle(title);

        Title = title.Trim();
        Description = description?.Trim();
        MarkAsUpdated(updatedById);
    }

    /// <summary>
    /// Business rule: Change task status with validation
    /// </summary>
    public void ChangeStatus(TaskStatus newStatus, int userId)
    {
        if (!CanTransitionTo(newStatus))
        {
            throw new InvalidTaskStatusTransitionException(
                Status.ToString(),
                newStatus.ToString());
        }

        var previousStatus = Status;
        Status = newStatus;
        MarkAsUpdated(userId);

        // Auto-set completion date
        if (newStatus == TaskStatus.Done)
        {
            CompletedAt = DateTime.UtcNow;
            Progress = 100;

            AddDomainEvent(new TaskCompletedEvent
            {
                TaskId = Id,
                CompletedById = userId,
                ActualHours = ActualHours
            });
        }

        AddDomainEvent(new TaskStatusChangedEvent
        {
            TaskId = Id,
            PreviousStatus = previousStatus.ToString(),
            NewStatus = newStatus.ToString(),
            ChangedById = userId
        });
    }

    /// <summary>
    /// Business rule: Assign task to user
    /// </summary>
    public void AssignTo(int? assigneeId, int assignedById)
    {
        var previousAssignee = AssigneeId;
        AssigneeId = assigneeId;
        MarkAsUpdated(assignedById);

        AddDomainEvent(new TaskAssignedEvent
        {
            TaskId = Id,
            PreviousAssigneeId = previousAssignee,
            NewAssigneeId = assigneeId,
            AssignedById = assignedById
        });
    }

    /// <summary>
    /// Business rule: Set task priority
    /// </summary>
    public void SetPriority(TaskPriority priority, int userId)
    {
        Priority = priority;
        MarkAsUpdated(userId);
    }

    /// <summary>
    /// Business rule: Update progress (0-100)
    /// </summary>
    public void UpdateProgress(double progress, int userId)
    {
        if (progress < 0 || progress > 100)
            throw new ArgumentException("Progress must be between 0 and 100");

        Progress = progress;
        MarkAsUpdated(userId);

        // Auto-complete if 100%
        if (progress >= 100 && Status != TaskStatus.Done)
        {
            ChangeStatus(TaskStatus.Done, userId);
        }
    }

    /// <summary>
    /// Business rule: Block task with reason
    /// </summary>
    public void Block(string reason, int userId)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Block reason is required");

        IsBlocked = true;
        BlockedReason = reason.Trim();
        MarkAsUpdated(userId);
    }

    /// <summary>
    /// Business rule: Unblock task
    /// </summary>
    public void Unblock(int userId)
    {
        IsBlocked = false;
        BlockedReason = null;
        MarkAsUpdated(userId);
    }

    /// <summary>
    /// Business rule: Set due date (must be in future)
    /// </summary>
    public void SetDueDate(DateTime? dueDate, int userId)
    {
        if (dueDate.HasValue && dueDate.Value < DateTime.UtcNow.Date)
            throw new ArgumentException("Due date cannot be in the past");

        DueDate = dueDate;
        MarkAsUpdated(userId);
    }

    /// <summary>
    /// Business rule: Set estimated hours
    /// </summary>
    public void SetEstimatedHours(int? hours, int userId)
    {
        if (hours.HasValue && hours.Value < 0)
            throw new ArgumentException("Estimated hours cannot be negative");

        EstimatedHours = hours;
        MarkAsUpdated(userId);
    }

    /// <summary>
    /// Business rule: Track actual hours spent
    /// </summary>
    public void SetActualHours(int? hours, int userId)
    {
        if (hours.HasValue && hours.Value < 0)
            throw new ArgumentException("Actual hours cannot be negative");

        ActualHours = hours;
        MarkAsUpdated(userId);
    }

    /// <summary>
    /// Business rule: Soft delete task
    /// </summary>
    public void Delete(int userId)
    {
        MarkAsDeleted(userId);

        AddDomainEvent(new TaskDeletedEvent
        {
            TaskId = Id,
            DeletedById = userId
        });
    }

    // Private validation methods
    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Task title cannot be empty");

        if (title.Length > 200)
            throw new ArgumentException("Task title cannot exceed 200 characters");
    }

    private bool CanTransitionTo(TaskStatus newStatus)
    {
        // Business rules for status transitions
        return (Status, newStatus) switch
        {
            (TaskStatus.Todo, TaskStatus.InProgress) => true,
            (TaskStatus.InProgress, TaskStatus.InReview) => true,
            (TaskStatus.InProgress, TaskStatus.Todo) => true,
            (TaskStatus.InReview, TaskStatus.Done) => true,
            (TaskStatus.InReview, TaskStatus.InProgress) => true,
            (TaskStatus.Done, TaskStatus.InProgress) => true, // Reopen
            (_, TaskStatus.Cancelled) => true, // Can cancel anytime
            (_, TaskStatus.Archived) => Status == TaskStatus.Done, // Only done tasks
            _ => false
        };
    }
}

public enum TaskStatus
{
    Todo = 0,
    InProgress = 1,
    InReview = 2,
    Done = 3,
    Archived = 4,
    Cancelled = 5
}

public enum TaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}
