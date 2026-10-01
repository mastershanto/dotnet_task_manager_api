using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.ResetPassword;

/// <summary>
/// নতুন পাসওয়ার্ড সেট করার কমান্ড (ওটিপি সহ):
/// </summary>
public record ResetPasswordCommand(
    string Email,
    string Otp,
    string NewPassword
) : ICommand<bool>;
