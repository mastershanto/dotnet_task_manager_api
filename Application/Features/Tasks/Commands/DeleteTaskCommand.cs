using MediatR;
using TodoApi.Application.Abstractions;
using TodoApi.Domain.Common;
using TodoApi.Models;
using TodoApi.Presentation.Common;
using Microsoft.Extensions.Logging;

namespace TodoApi.Application.Features.Tasks.Commands;

/// <summary>
/// Command to delete a task
/// </summary>
public sealed record DeleteTaskCommand(
    int TaskId,
    int DeletedById
) : IRequest<Result<Unit>>;

/// <summary>
/// Handler for DeleteTaskCommand
/// </summary>
public sealed class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result<Unit>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectAccessService _projectAccess;
    private readonly ILogger<DeleteTaskCommandHandler> _logger;

    public DeleteTaskCommandHandler(
        ITaskRepository taskRepository,
        IProjectAccessService projectAccess,
        ILogger<DeleteTaskCommandHandler> logger)
    {
        _taskRepository = taskRepository;
        _projectAccess = projectAccess;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var task = await _taskRepository.GetByIdAsync(command.TaskId);
            if (task == null)
                return Result<Unit>.Failure("Task not found");

            // Verify user has access to the project
            var hasAccess = await _projectAccess.UserHasProjectAccessAsync(task.ProjectId, command.DeletedById);
            if (!hasAccess)
                return Result<Unit>.Failure("You don't have permission to delete this task");

            // Soft delete - set DeletedAt timestamp
            task.DeletedAt = DateTime.UtcNow;
            await _taskRepository.SaveChangesAsync();

            _logger.LogInformation("Task {TaskId} deleted by user {UserId}", command.TaskId, command.DeletedById);

            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TaskId}", command.TaskId);
            return Result<Unit>.Failure($"Error deleting task: {ex.Message}");
        }
    }
}
