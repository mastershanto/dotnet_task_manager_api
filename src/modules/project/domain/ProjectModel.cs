using System.ComponentModel.DataAnnotations;

namespace Projects.Domain;

public enum ProjectStatus
{
    Planning = 1,
    Active = 2,
    OnHold = 3,
    Completed = 4,
    Archived = 5
}

/// <summary>
/// Project ডোমেন এন্টিটি (Domain Entity):
/// টাস্ক ম্যানেজারের প্রতিটি প্রজেক্টের কোর মডেল।
/// </summary>
public record ProjectModel
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string Description { get; init; } = string.Empty;

    public ProjectStatus Status { get; init; } = ProjectStatus.Active;

    [StringLength(50)]
    public string Color { get; init; } = "#3B82F6";

    public DateTime? StartDate { get; init; }

    public DateTime? EndDate { get; init; }

    public Guid? OwnerId { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; init; }
}
