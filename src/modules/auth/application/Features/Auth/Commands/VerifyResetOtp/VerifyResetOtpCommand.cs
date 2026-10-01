using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.VerifyResetOtp;

/// <summary>
/// পাসওয়ার্ড রিসেট ওটিপি যাচাই করার কমান্ড:
/// </summary>
public record VerifyResetOtpCommand(
    string Email,
    string Otp
) : ICommand<bool>;
