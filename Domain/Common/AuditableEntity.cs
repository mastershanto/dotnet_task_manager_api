namespace TodoApi.Domain.Common;

/// <summary>
/// Enterprise auditable entity with full audit trail
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public int CreatedById { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public int? UpdatedById { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedById { get; private set; }
    public bool IsDeleted => DeletedAt.HasValue;

    public void MarkAsCreated(int userId)
    {
        CreatedById = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsUpdated(int userId)
    {
        UpdatedById = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted(int userId)
    {
        DeletedById = userId;
        DeletedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        DeletedAt = null;
        DeletedById = null;
    }
}
