namespace TodoApi.Domain.Common;

/// <summary>
/// Base entity with domain events support - Enterprise DDD pattern
/// </summary>
public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public int Id { get; protected set; }
    
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
