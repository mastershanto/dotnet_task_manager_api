using MediatR;
using TodoApi.Domain.Common;
using TodoApi.Application.Features.Tasks.Commands.CreateTask;

namespace TodoApi.Application.Features.Tasks.Queries.GetProjectTasks;

/// <summary>
/// Query to get paginated tasks for a project - CQRS pattern
/// </summary>
public record GetProjectTasksQuery : IRequest<Result<PagedResult<TaskResponse>>>
{
    public int ProjectId { get; init; }
    public int UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; }
    public string? SearchTerm { get; init; }
    public int? StatusFilter { get; init; }
    public int? PriorityFilter { get; init; }
}

public record PagedResult<T>
{
    public List<T> Items { get; init; } = new();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
