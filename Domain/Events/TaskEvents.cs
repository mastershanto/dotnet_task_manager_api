using TodoApi.Domain.Common;

namespace TodoApi.Domain.Events;

/// <summary>
/// Domain event raised when a task is created
/// </summary>
public record TaskCreatedEvent : IDomainEvent
{
    public int TaskId { get; init; }
    public int ProjectId { get; init; }
    public int CreatedById { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Domain event raised when task status changes
/// </summary>
public record TaskStatusChangedEvent : IDomainEvent
{
    public int TaskId { get; init; }
    public string PreviousStatus { get; init; } = string.Empty;
    public string NewStatus { get; init; } = string.Empty;
    public int ChangedById { get; init; }
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Domain event raised when task is assigned
/// </summary>
public record TaskAssignedEvent : IDomainEvent
{
    public int TaskId { get; init; }
    public int? PreviousAssigneeId { get; init; }
    public int? NewAssigneeId { get; init; }
    public int AssignedById { get; init; }
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Domain event raised when task is completed
/// </summary>
public record TaskCompletedEvent : IDomainEvent
{
    public int TaskId { get; init; }
    public int CompletedById { get; init; }
    public int? ActualHours { get; init; }
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Domain event raised when task is deleted
/// </summary>
public record TaskDeletedEvent : IDomainEvent
{
    public int TaskId { get; init; }
    public int DeletedById { get; init; }
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
