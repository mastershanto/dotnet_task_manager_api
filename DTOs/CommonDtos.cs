namespace TodoApi.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = [];
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public class CreateTaskDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int ProjectId { get; set; }
    public int? AssigneeId { get; set; }
    public int Priority { get; set; } = 3;
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public int? EstimatedHours { get; set; }
    public string? Tags { get; set; }
    public int? ParentTaskId { get; set; }
}

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? AssigneeId { get; set; }
    public int? Status { get; set; }
    public int? Priority { get; set; }
    public double? Progress { get; set; }
    public DateTime? DueDate { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }
    public string? Tags { get; set; }
    public bool? IsBlocked { get; set; }
    public string? BlockedReason { get; set; }
}

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int ProjectId { get; set; }
    public int? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public int CreatedById { get; set; }
    public string? CreatedByName { get; set; }
    public int Status { get; set; }
    public string? StatusName { get; set; }
    public int Priority { get; set; }
    public string? PriorityName { get; set; }
    public double? Progress { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }
    public string? Tags { get; set; }
    public bool IsBlocked { get; set; }
    public string? BlockedReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CommentCount { get; set; }
    public int AttachmentCount { get; set; }
}

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

public class UpdateProjectDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public int? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public double? Progress { get; set; }
    public int? Priority { get; set; }
    public bool? IsPublic { get; set; }
}

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public int? TeamId { get; set; }
    public string? TeamName { get; set; }
    public string? Color { get; set; }
    public int Status { get; set; }
    public string? StatusName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public double? Progress { get; set; }
    public int Priority { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TaskCount { get; set; }
    public int MemberCount { get; set; }
}

public class CreateTeamDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Logo { get; set; }
    public int MaxMembers { get; set; } = 100;
}

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public string? Logo { get; set; }
    public int Status { get; set; }
    public int MaxMembers { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int MemberCount { get; set; }
    public int ProjectCount { get; set; }
}

public class TaskCommentDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public string Content { get; set; } = null!;
    public bool IsEdited { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? ParentCommentId { get; set; }
    public List<TaskCommentDto> Replies { get; set; } = [];
}

public class CreateCommentDto
{
    public string Content { get; set; } = null!;
    public int? ParentCommentId { get; set; }
}

public class LoginDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegisterDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? FullName { get; set; }
}

public class AuthResponseDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Avatar { get; set; }
    public int Status { get; set; }
    public string[] Roles { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class QueryParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public string? SearchTerm { get; set; }
    public int? Status { get; set; }
    public int? Priority { get; set; }
}
