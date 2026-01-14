using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Domain.Exceptions;
using TodoApi.Domain.Common;
using TodoApi.Models;

namespace TodoApi.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// Handler for CreateTask command - CQRS pattern with business logic
/// </summary>
public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<TaskResponse>>
{
    private readonly TodoContext _context;
    private readonly ILogger<CreateTaskCommandHandler> _logger;

    public CreateTaskCommandHandler(TodoContext context, ILogger<CreateTaskCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<TaskResponse>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verify project exists
            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == request.ProjectId && !p.DeletedAt.HasValue, cancellationToken);

            if (!projectExists)
                return Result<TaskResponse>.Failure($"Project {request.ProjectId} not found");

            // Verify assignee exists if provided
            if (request.AssigneeId.HasValue)
            {
                var assigneeExists = await _context.Users
                    .AnyAsync(u => u.Id == request.AssigneeId.Value, cancellationToken);

                if (!assigneeExists)
                    return Result<TaskResponse>.Failure($"Assignee {request.AssigneeId} not found");
            }

            // TODO: Replace hard-coded userId with current user context (ICurrentUserService)
            var userId = 1;

            // Create task using the current EF Core entity model (TodoApi.Models.TaskItem)
            var task = new TaskItem
            {
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                ProjectId = request.ProjectId,
                AssigneeId = request.AssigneeId,
                CreatedById = userId,
                Priority = (TaskPriority)request.Priority,
                DueDate = request.DueDate,
                EstimatedHours = request.EstimatedHours,
                Tags = request.Tags,
                ParentTaskId = request.ParentTaskId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Task {TaskId} created successfully in project {ProjectId}", 
                task.Id, request.ProjectId);

            // Map to response
            var response = new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                ProjectId = task.ProjectId,
                AssigneeId = task.AssigneeId,
                Status = (int)task.Status,
                StatusName = task.Status.ToString(),
                Priority = (int)task.Priority,
                PriorityName = task.Priority.ToString(),
                Progress = task.Progress,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt
            };

            return Result<TaskResponse>.Success(response);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain exception while creating task");
            return Result<TaskResponse>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return Result<TaskResponse>.Failure("An error occurred while creating the task");
        }
    }
}
