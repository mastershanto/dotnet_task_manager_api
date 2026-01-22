namespace TodoApi.Presentation.DTOs;

/// <summary>
/// DTO for creating a project
/// </summary>
public class CreateProjectDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int? TeamId { get; set; }
    public string? Color { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int Priority { get; set; } = 3;
    public bool IsPublic { get; set; }
}

/// <summary>
/// DTO for updating a project
/// </summary>
public class UpdateProjectDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public int? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int? Priority { get; set; }
    public bool? IsPublic { get; set; }
}

/// <summary>
/// DTO for returning project details
/// </summary>
public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int CreatedById { get; set; }
    public int? TeamId { get; set; }
    public int Status { get; set; }
    public string? StatusName { get; set; }
    public int Priority { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TaskCount { get; set; }
    public int CompletedTaskCount { get; set; }
}
