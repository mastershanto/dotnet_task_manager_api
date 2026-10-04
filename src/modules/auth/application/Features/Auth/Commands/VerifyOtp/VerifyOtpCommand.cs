using Auth.Domain;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.VerifyOtp;

public record VerifyOtpCommand(string Email, string Otp) : ICommand<OtpResult>;
