namespace Auth.Application.Models;

/// <summary>
/// পাসওয়ার্ড ভুলে যাওয়ার রিকোয়েস্টে পাঠানো রেসপন্স DTO
/// </summary>
public record ForgotPasswordResponse(
    bool Success,
    string Message,
    string Email,
    string? Otp = null
);
