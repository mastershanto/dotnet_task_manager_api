using Auth.Domain;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.VerifyRegistrationOtp;

/// <summary>
/// রেজিস্ট্রেশন ওটিপি যাচাই করার কমান্ড:
/// </summary>
public record VerifyRegistrationOtpCommand(
    string Email,
    string Otp
) : ICommand<AuthResponse>;
