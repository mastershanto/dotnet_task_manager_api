namespace TodoApi.Models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    public string? Logo { get; set; }
    public TeamStatus Status { get; set; } = TeamStatus.Active;
    public int MaxMembers { get; set; } = 100;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<TeamMember> Members { get; set; } = [];
    public ICollection<Project> Projects { get; set; } = [];
}

public enum TeamStatus
{
    Active,
    Suspended,
    Deleted
}

public class TeamMember
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public TeamRole Role { get; set; } = TeamRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public string? JoinedBy { get; set; }
}

public enum TeamRole
{
    Owner,
    Admin,
    Member,
    Viewer
}
