using MediatR;
using TodoApi.Domain.Events;
using Microsoft.Extensions.Logging;

namespace TodoApi.Application.Features.Tasks.DomainEventHandlers;

/// <summary>
/// Domain event handler for when a task is assigned
/// Useful for team notifications and workload management
/// </summary>
public sealed class TaskAssignedEventHandler : INotificationHandler<TaskAssignedEvent>
{
    private readonly ILogger<TaskAssignedEventHandler> _logger;

    public TaskAssignedEventHandler(ILogger<TaskAssignedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TaskAssignedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Task assigned event occurred: TaskId={TaskId}, PreviousAssignee={PreviousAssigneeId}, NewAssignee={NewAssigneeId}, AssignedBy={AssignedById}",
                notification.TaskId,
                notification.PreviousAssigneeId ?? 0,
                notification.NewAssigneeId ?? 0,
                notification.AssignedById
            );

            // Additional operations:
            // - Send notification to newly assigned team member
            // - Update team member's task load
            // - Notify previous assignee of reassignment
            // - Update workload metrics
            // - Send notifications to project stakeholders

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling TaskAssignedEvent for task {TaskId}", notification.TaskId);
            throw;
        }
    }
}

