namespace Auth.Application.Models;

/// <summary>
/// প্রমাণীকরণ সফল হলে ক্লায়েন্টকে পাঠানো রেসপন্স DTO
/// </summary>
public record AuthResponse(
    bool Success,
    string Message,
    string? Token,
    Guid? UserId = null,
    string? Name = null,
    string? Email = null,
    string? Role = null
);
