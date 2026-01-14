using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ICurrentUserService currentUser, ILogger<TasksController> logger)
    {
        _taskService = taskService;
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
            var task = await _taskService.GetTaskByIdAsync(taskId, userId);
            return Ok(new ApiResponse<TaskDto>
            {
                Success = true,
                Message = "Task retrieved successfully",
                Data = task
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Task {TaskId} not found", taskId);
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get all tasks in a project with pagination and filtering
    /// </summary>
    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<TaskDto>>>> GetProjectTasks(
        int projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? status = null,
        [FromQuery] int? priority = null)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var queryParams = new QueryParams
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDescending = sortDescending,
                SearchTerm = searchTerm,
                Status = status,
                Priority = priority
            };

            var tasks = await _taskService.GetTasksAsync(projectId, userId, queryParams);
            return Ok(new ApiResponse<PaginatedResponse<TaskDto>>
            {
                Success = true,
                Message = "Tasks retrieved successfully",
                Data = tasks
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get all tasks assigned to the current user
    /// </summary>
    [HttpGet("assigned-to-me")]
    public async Task<ActionResult<ApiResponse<List<TaskDto>>>> GetMyTasks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = GetUserIdOrThrow();
        var queryParams = new QueryParams
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var tasks = await _taskService.GetUserTasksAsync(userId, queryParams);
        return Ok(new ApiResponse<List<TaskDto>>
        {
            Success = true,
            Message = "Your tasks retrieved successfully",
            Data = tasks
        });
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
            var task = await _taskService.CreateTaskAsync(dto, userId);
            return CreatedAtAction(nameof(GetTask), new { taskId = task.Id }, new ApiResponse<TaskDto>
            {
                Success = true,
                Message = "Task created successfully",
                Data = task
            });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
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
            var task = await _taskService.UpdateTaskAsync(taskId, dto, userId);
            return Ok(new ApiResponse<TaskDto>
            {
                Success = true,
                Message = "Task updated successfully",
                Data = task
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
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
            await _taskService.DeleteTaskAsync(taskId, userId);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Task deleted successfully"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Change task status
    /// </summary>
    [HttpPatch("{taskId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> ChangeTaskStatus(int taskId, [FromQuery] int status)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            await _taskService.ChangeTaskStatusAsync(taskId, status, userId);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Task status updated successfully"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Assign task to a user
    /// </summary>
    [HttpPatch("{taskId}/assign")]
    public async Task<ActionResult<ApiResponse<object>>> AssignTask(int taskId, [FromQuery] int? assigneeId)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            await _taskService.AssignTaskAsync(taskId, assigneeId, userId);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Task assigned successfully"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }
}
