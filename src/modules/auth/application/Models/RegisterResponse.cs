namespace Auth.Application.Models;

/// <summary>
/// রেজিস্ট্রেশন সফল হলে ক্লায়েন্টকে পাঠানো রেসপন্স DTO
/// </summary>
public record RegisterResponse(
    bool Success,
    string Message,
    string Email,
    string? Otp = null
);
