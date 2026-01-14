using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Application.Features.Tasks.Commands.CreateTask;
using TodoApi.Application.Features.Tasks.Queries.GetTaskById;
using TodoApi.Application.Features.Tasks.Queries.GetProjectTasks;

namespace TodoApi.Controllers;

/// <summary>
/// Enterprise Tasks Controller using CQRS pattern
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tasks")]
[Produces("application/json")]
[Authorize]
public class TasksV2Controller : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TasksV2Controller> _logger;

    public TasksV2Controller(IMediator mediator, ILogger<TasksV2Controller> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    /// <param name="command">Task creation data</param>
    /// <returns>Created task</returns>
    /// <response code="201">Task created successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new
            {
                success = false,
                message = result.Error,
                errors = result.Errors
            });
        }

        return CreatedAtAction(
            nameof(GetTask),
            new { taskId = result.Value!.Id },
            new
            {
                success = true,
                message = "Task created successfully",
                data = result.Value
            });
    }

    /// <summary>
    /// Get a task by ID
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <returns>Task details</returns>
    /// <response code="200">Task found</response>
    /// <response code="404">Task not found</response>
    [HttpGet("{taskId}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTask(int taskId)
    {
        var query = new GetTaskByIdQuery { TaskId = taskId, UserId = 1 }; // TODO: Get from auth context
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new
            {
                success = false,
                message = result.Error
            });
        }

        return Ok(new
        {
            success = true,
            message = "Task retrieved successfully",
            data = result.Value
        });
    }

    /// <summary>
    /// Get all tasks in a project with pagination
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortDescending">Sort descending</param>
    /// <param name="searchTerm">Search term</param>
    /// <param name="statusFilter">Status filter</param>
    /// <param name="priorityFilter">Priority filter</param>
    /// <returns>Paginated task list</returns>
    [HttpGet("project/{projectId}")]
    [ProducesResponseType(typeof(PagedResult<TaskResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectTasks(
        int projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? statusFilter = null,
        [FromQuery] int? priorityFilter = null)
    {
        var query = new GetProjectTasksQuery
        {
            ProjectId = projectId,
            UserId = 1, // TODO: Get from auth context
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending,
            SearchTerm = searchTerm,
            StatusFilter = statusFilter,
            PriorityFilter = priorityFilter
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        return Ok(new
        {
            success = true,
            message = "Tasks retrieved successfully",
            data = result.Value
        });
    }
}
