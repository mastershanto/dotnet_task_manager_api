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
/// Query to retrieve paginated tasks for a project
/// </summary>
public sealed record GetProjectTasksQuery(
    int ProjectId,
    int RequestedById,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = null,
    bool SortDescending = false
) : IRequest<Result<PresentationCommon.PaginatedResponse<TaskSummaryDto>>>;

/// <summary>
/// Handler for GetProjectTasksQuery
/// </summary>
public sealed class GetProjectTasksQueryHandler : IRequestHandler<GetProjectTasksQuery, Result<PresentationCommon.PaginatedResponse<TaskSummaryDto>>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectAccessService _projectAccess;
    private readonly ILogger<GetProjectTasksQueryHandler> _logger;

    public GetProjectTasksQueryHandler(
        ITaskRepository taskRepository,
        IProjectAccessService projectAccess,
        ILogger<GetProjectTasksQueryHandler> logger)
    {
        _taskRepository = taskRepository;
        _projectAccess = projectAccess;
        _logger = logger;
    }

    public async Task<Result<PresentationCommon.PaginatedResponse<TaskSummaryDto>>> Handle(GetProjectTasksQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var hasAccess = await _projectAccess.UserHasProjectAccessAsync(query.ProjectId, query.RequestedById);
            if (!hasAccess)
                return Result<PresentationCommon.PaginatedResponse<TaskSummaryDto>>.Failure("You don't have access to this project");

            // Create QueryParams for repository - use object initializer compatible with interface
            var queryParams = new QueryParams
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                SortBy = query.SortBy,
                SortDescending = query.SortDescending
            };

            var (tasks, totalCount) = await _taskRepository.GetProjectTasksAsync(query.ProjectId, queryParams);

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
            _logger.LogError(ex, "Error retrieving project tasks for project {ProjectId}", query.ProjectId);
            return Result<PresentationCommon.PaginatedResponse<TaskSummaryDto>>.Failure($"Error retrieving tasks: {ex.Message}");
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
            AssigneeId = task.AssigneeId,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt
        };
    }
}
