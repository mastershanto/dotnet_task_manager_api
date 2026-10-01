using Auth.Domain;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.ForgotPassword;

/// <summary>
/// পাসওয়ার্ড ভুলে গেলে ওটিপি চাওয়ার কমান্ড:
/// </summary>
public record ForgotPasswordCommand(
    string Email
) : ICommand<ForgotPasswordResponse>;
