namespace TodoApi.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public string[] Roles { get; set; } = new[] { "User" };
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation
    public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    public ICollection<TeamMember> TeamMemberships { get; set; } = new List<TeamMember>();
}

public enum UserStatus
{
    Active,
    Inactive,
    Suspended,
    Deleted
}
