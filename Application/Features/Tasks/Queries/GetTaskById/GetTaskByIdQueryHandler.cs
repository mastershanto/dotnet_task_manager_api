using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApi.Application.Features.Tasks.Commands.CreateTask;
using TodoApi.Data;
using TodoApi.Domain.Common;

namespace TodoApi.Application.Features.Tasks.Queries.GetTaskById;

/// <summary>
/// Handler for GetTaskById query - CQRS read pattern
/// </summary>
public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskResponse>>
{
    private readonly TodoContext _context;
    private readonly ILogger<GetTaskByIdQueryHandler> _logger;

    public GetTaskByIdQueryHandler(TodoContext context, ILogger<GetTaskByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<TaskResponse>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(t => t.Assignee)
            .Include(t => t.CreatedBy)
            .Include(t => t.Project)
            .Where(t => t.Id == request.TaskId && !t.DeletedAt.HasValue)
            .Select(t => new TaskResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                ProjectId = t.ProjectId,
                AssigneeId = t.AssigneeId,
                AssigneeName = t.Assignee != null ? t.Assignee.FullName ?? t.Assignee.Username : "",
                Status = (int)t.Status,
                StatusName = t.Status.ToString(),
                Priority = (int)t.Priority,
                PriorityName = t.Priority.ToString(),
                Progress = t.Progress,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                CreatedByName = t.CreatedBy.FullName ?? t.CreatedBy.Username
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (task == null)
        {
            _logger.LogWarning("Task {TaskId} not found", request.TaskId);
            return Result<TaskResponse>.Failure($"Task {request.TaskId} not found");
        }

        return Result<TaskResponse>.Success(task);
    }
}
