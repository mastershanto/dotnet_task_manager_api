using System.ComponentModel.DataAnnotations;

namespace Tasks.Domain;

public enum TaskItemStatus
{
    Todo = 1,
    InProgress = 2,
    Completed = 3
}

public enum TaskPriority
{
    Low = 1,
    Medium = 2,
    High = 3
}

/// <summary>
/// Task ডোমেন এন্টিটি (Domain Entity):
/// Clean Architecture অনুযায়ী ডোমেন মডেলটি পিওর এবং কোনো ফ্রেমওয়ার্ক ডিপেন্ডেন্সি মুক্ত।
/// </summary>
public record TaskItemModel
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Title { get; init; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; init; } = string.Empty;

    public TaskItemStatus Status { get; init; } = TaskItemStatus.Todo;

    public TaskPriority Priority { get; init; } = TaskPriority.Medium;

    public DateTime? DueDate { get; init; }

    public Guid? CategoryId { get; init; }

    public Guid? AssignedUserId { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; init; }
}
