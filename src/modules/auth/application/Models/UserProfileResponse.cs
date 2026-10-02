namespace Auth.Application.Models;

/// <summary>
/// ইউজারের প্রোফাইল তথ্য সংক্রান্ত রেসপন্স DTO
/// </summary>
public record UserProfileResponse(
    Guid Id,
    string Name,
    string Email,
    string Role,
    bool IsEmailVerified,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
