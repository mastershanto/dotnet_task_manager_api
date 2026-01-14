using MediatR;

namespace TodoApi.Domain.Common;

/// <summary>
/// Marker interface for domain events
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
