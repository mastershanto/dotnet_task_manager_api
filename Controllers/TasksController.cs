using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Application.Features.Tasks.Commands;
using TodoApi.Application.Features.Tasks.Queries;
using TodoApi.Domain.Entities;
using TodoApi.Presentation.Common;
using TodoApi.Presentation.DTOs;
using TodoApi.Services;

namespace TodoApi.Controllers;

/// <summary>
/// Tasks API controller - Enterprise clean architecture with CQRS pattern
/// Delegates all business logic to MediatR handlers through commands and queries
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<TasksController> _logger;

    public TasksController(IMediator mediator, ICurrentUserService currentUser, ILogger<TasksController> logger)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _logger = logger;
    }

    private int GetUserIdOrThrow()
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
            throw new UnauthorizedAccessException("Unauthorized");

        return _currentUser.UserId.Value;
    }

    /// <summary>
    /// Get a specific task by ID
    /// </summary>
    [HttpGet("{taskId}")]
    public async Task<ActionResult<ApiResponse<TaskDto>>> GetTask(int taskId)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var query = new GetTaskByIdQuery(taskId, userId);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(new ApiResponse<object> { Success = false, Message = result.Error });

            return Ok(new ApiResponse<TaskDto>
            {
                Success = true,
                Message = result.Message,
                Data = result.Value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task {TaskId}", taskId);
            return StatusCode(500, new ApiResponse<object> { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get all tasks in a project with pagination
    /// </summary>
    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<TaskSummaryDto>>>> GetProjectTasks(
        int projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var query = new GetProjectTasksQuery(projectId, userId, pageNumber, pageSize, sortBy, sortDescending);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(new ApiResponse<object> { Success = false, Message = result.Error });

            return Ok(new ApiResponse<PaginatedResponse<TaskSummaryDto>>
            {
                Success = true,
                Message = result.Message,
                Data = result.Value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project tasks for project {ProjectId}", projectId);
            return StatusCode(500, new ApiResponse<object> { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get all tasks assigned to the current user
    /// </summary>
    [HttpGet("assigned-to-me")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<TaskSummaryDto>>>> GetMyTasks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var query = new GetUserTasksQuery(userId, pageNumber, pageSize);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(new ApiResponse<object> { Success = false, Message = result.Error });

            return Ok(new ApiResponse<PaginatedResponse<TaskSummaryDto>>
            {
                Success = true,
                Message = result.Message,
                Data = result.Value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user tasks");
            return StatusCode(500, new ApiResponse<object> { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TaskDto>>> CreateTask([FromBody] CreateTaskDto dto)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var command = new CreateTaskCommand(
                dto.Title,
                dto.Description,
                dto.ProjectId,
                dto.AssigneeId,
                dto.Priority,
                dto.DueDate,
                dto.StartDate,
                dto.EstimatedHours,
                dto.Tags,
                dto.ParentTaskId,
                userId
            );

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new ApiResponse<object> { Success = false, Message = result.Error });

            return CreatedAtAction(nameof(GetTask), new { taskId = result.Value?.Id }, new ApiResponse<TaskDto>
            {
                Success = true,
                Message = result.Message,
                Data = result.Value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return StatusCode(500, new ApiResponse<object> { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    [HttpPut("{taskId}")]
    public async Task<ActionResult<ApiResponse<TaskDto>>> UpdateTask(int taskId, [FromBody] UpdateTaskDto dto)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var command = new UpdateTaskCommand(
                taskId,
                dto.Title,
                dto.Description,
                dto.AssigneeId,
                dto.Status,
                dto.Priority,
                dto.Progress,
                dto.DueDate,
                dto.EstimatedHours,
                dto.ActualHours,
                dto.Tags,
                dto.IsBlocked,
                dto.BlockedReason,
                userId
            );

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new ApiResponse<object> { Success = false, Message = result.Error });

            return Ok(new ApiResponse<TaskDto>
            {
                Success = true,
                Message = result.Message,
                Data = result.Value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task {TaskId}", taskId);
            return StatusCode(500, new ApiResponse<object> { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    [HttpDelete("{taskId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTask(int taskId)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var command = new DeleteTaskCommand(taskId, userId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new ApiResponse<object> { Success = false, Message = result.Error });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = result.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TaskId}", taskId);
            return StatusCode(500, new ApiResponse<object> { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Change task status
    /// </summary>
    [HttpPatch("{taskId}/status")]
    public async Task<ActionResult<ApiResponse<TaskDto>>> ChangeTaskStatus(int taskId, [FromBody] ChangeTaskStatusDto dto)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            if (!Enum.TryParse<TaskStatus>(dto.NewStatus.ToString(), out var newStatus))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid status value" });

            var command = new ChangeTaskStatusCommand(taskId, newStatus, userId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new ApiResponse<object> { Success = false, Message = result.Error });

            return Ok(new ApiResponse<TaskDto>
            {
                Success = true,
                Message = result.Message,
                Data = result.Value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing task status for {TaskId}", taskId);
            return StatusCode(500, new ApiResponse<object> { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Assign task to a user
    /// </summary>
    [HttpPatch("{taskId}/assign")]
    public async Task<ActionResult<ApiResponse<TaskDto>>> AssignTask(int taskId, [FromBody] AssignTaskDto dto)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var command = new AssignTaskCommand(taskId, dto.AssigneeId, userId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new ApiResponse<object> { Success = false, Message = result.Error });

            return Ok(new ApiResponse<TaskDto>
            {
                Success = true,
                Message = result.Message,
                Data = result.Value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning task {TaskId}", taskId);
            return StatusCode(500, new ApiResponse<object> { Success = false, Message = "Internal server error" });
        }
    }
}
