using MediatR;
using TodoApi.Domain.Common;

namespace TodoApi.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// Command to create a new task - CQRS pattern
/// </summary>
public record CreateTaskCommand : IRequest<Result<TaskResponse>>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int ProjectId { get; init; }
    public int? AssigneeId { get; init; }
    public int Priority { get; init; } = 1; // Medium
    public DateTime? DueDate { get; init; }
    public int? EstimatedHours { get; init; }
    public int? ParentTaskId { get; init; }
    public string? Tags { get; init; }
}

public record TaskResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int ProjectId { get; init; }
    public int? AssigneeId { get; init; }
    public string AssigneeName { get; init; } = string.Empty;
    public int Status { get; init; }
    public string StatusName { get; init; } = string.Empty;
    public int Priority { get; init; }
    public string PriorityName { get; init; } = string.Empty;
    public double? Progress { get; init; }
    public DateTime? DueDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
}
