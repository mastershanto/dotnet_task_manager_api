using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.DeleteAccount;

/// <summary>
/// একাউন্ট স্থায়ীভাবে ডিলিট করার কমান্ড:
/// </summary>
public record DeleteAccountCommand(
    Guid UserId,
    string Password
) : ICommand<bool>;
