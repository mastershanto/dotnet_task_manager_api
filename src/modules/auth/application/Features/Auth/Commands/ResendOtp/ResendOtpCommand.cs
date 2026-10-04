using Auth.Domain;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.ResendOtp;

public record ResendOtpCommand(string Email) : ICommand<RegisterResult>;
