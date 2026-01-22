using MediatR;
using TodoApi.Application.Abstractions;
using TodoApi.Domain.Events;
using Microsoft.Extensions.Logging;

namespace TodoApi.Application.Features.Tasks.DomainEventHandlers;

/// <summary>
/// Domain event handler for when a task is created
/// Can be extended to: notify team members, create activity logs, send webhooks, etc.
/// </summary>
public sealed class TaskCreatedEventHandler : INotificationHandler<TaskCreatedEvent>
{
    private readonly ILogger<TaskCreatedEventHandler> _logger;

    public TaskCreatedEventHandler(ILogger<TaskCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TaskCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Task created event occurred: TaskId={TaskId}, ProjectId={ProjectId}, CreatedBy={CreatedById}",
                notification.TaskId,
                notification.ProjectId,
                notification.CreatedById
            );

            // Additional operations can be added here:
            // - Send notifications to team members
            // - Create activity feed entries
            // - Trigger webhooks
            // - Update project metrics
            // - Send to message queue for async processing

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling TaskCreatedEvent for task {TaskId}", notification.TaskId);
            throw;
        }
    }
}
