namespace TodoApi.Presentation.DTOs;

/// <summary>
/// DTO for creating a new task
/// </summary>
public class CreateTaskDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int ProjectId { get; set; }
    public int? AssigneeId { get; set; }
    public int Priority { get; set; } = 3;
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public int? EstimatedHours { get; set; }
    public string? Tags { get; set; }
    public int? ParentTaskId { get; set; }
}

/// <summary>
/// DTO for updating a task
/// </summary>
public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? AssigneeId { get; set; }
    public int? Status { get; set; }
    public int? Priority { get; set; }
    public double? Progress { get; set; }
    public DateTime? DueDate { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }
    public string? Tags { get; set; }
    public bool? IsBlocked { get; set; }
    public string? BlockedReason { get; set; }
}

/// <summary>
/// DTO for task status change
/// </summary>
public class ChangeTaskStatusDto
{
    public int NewStatus { get; set; }
}

/// <summary>
/// DTO for assigning a task
/// </summary>
public class AssignTaskDto
{
    public int? AssigneeId { get; set; }
}

/// <summary>
/// DTO for returning task details
/// </summary>
public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int ProjectId { get; set; }
    public int? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public int CreatedById { get; set; }
    public string? CreatedByName { get; set; }
    public int Status { get; set; }
    public string? StatusName { get; set; }
    public int Priority { get; set; }
    public string? PriorityName { get; set; }
    public double? Progress { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }
    public string? Tags { get; set; }
    public bool IsBlocked { get; set; }
    public string? BlockedReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CommentCount { get; set; }
    public int AttachmentCount { get; set; }
}

/// <summary>
/// DTO for brief task summary (used in lists)
/// </summary>
public class TaskSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int Status { get; set; }
    public string? StatusName { get; set; }
    public int Priority { get; set; }
    public string? PriorityName { get; set; }
    public int? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
