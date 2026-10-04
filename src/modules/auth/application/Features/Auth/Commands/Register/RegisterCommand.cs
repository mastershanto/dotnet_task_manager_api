using Auth.Domain;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Name,
    string Email,
    string Password,
    string Type,
    bool AgreeToTerms,
    bool IsFullProgram
) : ICommand<RegisterResult>;
