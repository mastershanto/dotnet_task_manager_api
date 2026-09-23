using Auth.Domain;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : ICommand<AuthResult>;
