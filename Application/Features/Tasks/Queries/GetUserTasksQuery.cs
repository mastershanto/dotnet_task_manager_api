using MediatR;
using TodoApi.Application.Abstractions;
using TodoApi.Domain.Common;
using QueryParams = TodoApi.DTOs.QueryParams;
using TodoApi.Models;
using TodoApi.Presentation.DTOs;
using Microsoft.Extensions.Logging;
using PresentationCommon = TodoApi.Presentation.Common;

namespace TodoApi.Application.Features.Tasks.Queries;

/// <summary>
/// Query to retrieve all tasks assigned to a user
/// </summary>
public sealed record GetUserTasksQuery(
    int UserId,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = null,
    bool SortDescending = false
) : IRequest<Result<PresentationCommon.PaginatedResponse<TaskSummaryDto>>>;

/// <summary>
/// Handler for GetUserTasksQuery
/// </summary>
public sealed class GetUserTasksQueryHandler : IRequestHandler<GetUserTasksQuery, Result<PresentationCommon.PaginatedResponse<TaskSummaryDto>>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<GetUserTasksQueryHandler> _logger;

    public GetUserTasksQueryHandler(
        ITaskRepository taskRepository,
        ILogger<GetUserTasksQueryHandler> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<Result<PresentationCommon.PaginatedResponse<TaskSummaryDto>>> Handle(GetUserTasksQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var queryParams = new QueryParams
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                SortBy = query.SortBy,
                SortDescending = query.SortDescending
            };

            var tasks = await _taskRepository.GetUserTasksAsync(query.UserId, queryParams);
            var totalCount = tasks.Count;

            var response = new PresentationCommon.PaginatedResponse<TaskSummaryDto>
            {
                Items = tasks.Select(MapToTaskSummaryDto).ToList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            return Result<PresentationCommon.PaginatedResponse<TaskSummaryDto>>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for user {UserId}", query.UserId);
            return Result<PresentationCommon.PaginatedResponse<TaskSummaryDto>>.Failure($"Error retrieving user tasks: {ex.Message}");
        }
    }

    private TaskSummaryDto MapToTaskSummaryDto(TaskItem task)
    {
        return new TaskSummaryDto
        {
            Id = task.Id,
            Title = task.Title,
            Status = (int)task.Status,
            StatusName = task.Status.ToString(),
            Priority = (int)task.Priority,
            PriorityName = task.Priority.ToString(),
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt
        };
    }
}
