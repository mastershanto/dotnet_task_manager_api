namespace TodoApi.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int? AssigneeId { get; set; }
    public User? Assignee { get; set; }
    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public double? Progress { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }
    public string? Tags { get; set; } // JSON or comma-separated
    public int OrderIndex { get; set; }
    public bool IsBlocked { get; set; }
    public string? BlockedReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public ICollection<TaskComment> Comments { get; set; } = [];
    public ICollection<TaskAttachment> Attachments { get; set; } = [];
    public ICollection<TaskHistory> History { get; set; } = [];
    public ICollection<TaskItem> Subtasks { get; set; } = [];
    public int? ParentTaskId { get; set; }
    public TaskItem? ParentTask { get; set; }
}

public enum TaskStatus
{
    Todo,
    InProgress,
    InReview,
    Done,
    Archived,
    Cancelled
}

public enum TaskPriority
{
    Critical = 1,
    High = 2,
    Medium = 3,
    Low = 4
}

public class TaskComment
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public TaskItem Task { get; set; } = null!;
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsEdited { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? ParentCommentId { get; set; }
    public TaskComment? ParentComment { get; set; }
    public ICollection<TaskComment> Replies { get; set; } = [];
}

public class TaskAttachment
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public TaskItem Task { get; set; } = null!;
    public int UploadedById { get; set; }
    public User UploadedBy { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
}

public class TaskHistory
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public TaskItem Task { get; set; } = null!;
    public int ChangedById { get; set; }
    public User ChangedBy { get; set; } = null!;
    public string FieldName { get; set; } = null!;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
