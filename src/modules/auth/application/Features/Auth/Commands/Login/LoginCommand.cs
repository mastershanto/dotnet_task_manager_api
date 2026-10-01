using Auth.Domain;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.Login;

/// <summary>
/// লগইন কমান্ড (CQRS Command):
/// </summary>
public record LoginCommand(
    string Email,
    string Password
) : ICommand<AuthResponse>;
