namespace Auth.Application.Models;

/// <summary>
/// প্রোফাইল আপডেট রিকোয়েস্ট DTO
/// </summary>
public record UpdateProfileRequest(string Name);

/// <summary>
/// পাসওয়ার্ড পরিবর্তন রিকোয়েস্ট DTO
/// </summary>
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

/// <summary>
/// একাউন্ট ডিলিট রিকোয়েস্ট DTO
/// </summary>
public record DeleteAccountRequest(string Password);
