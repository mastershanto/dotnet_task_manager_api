namespace TodoApi.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    public int? TeamId { get; set; }
    public Team? Team { get; set; }
    public string? Color { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public double? Progress { get; set; }
    public int Priority { get; set; } = 3; // 1=Critical, 2=High, 3=Medium, 4=Low
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public ICollection<TaskItem> Tasks { get; set; } = [];
    public ICollection<ProjectMember> Members { get; set; } = [];
}

public enum ProjectStatus
{
    Planning,
    Active,
    OnHold,
    Completed,
    Archived,
    Deleted
}

public class ProjectMember
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ProjectRole Role { get; set; } = ProjectRole.Editor;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

public enum ProjectRole
{
    Owner,
    Admin,
    Editor,
    Viewer
}
