using Auth.Domain;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.PasswordReset;

public record PasswordResetRequestCommand(string Email) : ICommand<PasswordResetResult>;
public record PasswordResetResendCommand(string Email) : ICommand<PasswordResetResult>;
public record PasswordResetVerifyCommand(string Email, string Otp) : ICommand<PasswordResetResult>;
public record PasswordResetConfirmCommand(string ResetToken, string Password) : ICommand<PasswordResetResult>;
