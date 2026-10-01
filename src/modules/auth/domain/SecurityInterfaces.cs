using Users.Domain;

namespace Auth.Domain;

/// <summary>
/// পাসওয়ার্ড সিকিউরিটি সার্ভিস ইন্টারফেস (PBKDF2 / SHA-256 Hashing):
/// </summary>
public interface IPasswordHasherService
{
    string HashPassword(string plainPassword);
    bool VerifyPassword(string plainPassword, string passwordHash);
}

/// <summary>
/// JWT টোকেন জেনারেটর সার্ভিস ইন্টারফেস:
/// </summary>
public interface ITokenService
{
    string GenerateToken(UserModel user);
}

/// <summary>
/// ওটিপি জেনারেশন ও ভ্যালিডেশন সার্ভিস ইন্টারফেস:
/// </summary>
public interface IOtpService
{
    string GenerateOtp();
}

/// <summary>
/// প্রমাণীকরণ রেসপন্স DTOs:
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

public record RegisterResponse(
    bool Success,
    string Message,
    string Email,
    string? Otp = null // সাময়িকভাবে টেস্টিংয়ের জন্য রেসপন্সে ওটিপি পাঠানো হচ্ছে (ইউজারের নির্দেশ অনুযায়ী)
);

public record ForgotPasswordResponse(
    bool Success,
    string Message,
    string Email,
    string? Otp = null // সাময়িকভাবে টেস্টিংয়ের জন্য রেসপন্সে ওটিপি পাঠানো হচ্ছে (ইউজারের নির্দেশ অনুযায়ী)
);

public record UserProfileResponse(
    Guid Id,
    string Name,
    string Email,
    string Role,
    bool IsEmailVerified,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
