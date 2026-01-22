using MediatR;
using TodoApi.Domain.Events;
using Microsoft.Extensions.Logging;

namespace TodoApi.Application.Features.Tasks.DomainEventHandlers;

/// <summary>
/// Domain event handler for when task status changes
/// Useful for notifications, workflow automation, and audit trails
/// </summary>
public sealed class TaskStatusChangedEventHandler : INotificationHandler<TaskStatusChangedEvent>
{
    private readonly ILogger<TaskStatusChangedEventHandler> _logger;

    public TaskStatusChangedEventHandler(ILogger<TaskStatusChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TaskStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Task status changed event occurred: TaskId={TaskId}, PreviousStatus={PreviousStatus}, NewStatus={NewStatus}, ChangedBy={ChangedById}",
                notification.TaskId,
                notification.PreviousStatus,
                notification.NewStatus,
                notification.ChangedById
            );

            // Additional operations:
            // - Notify stakeholders of completion/blocking
            // - Trigger dependent workflows
            // - Update project progress
            // - Send status change notifications

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling TaskStatusChangedEvent for task {TaskId}", notification.TaskId);
            throw;
        }
    }
}

