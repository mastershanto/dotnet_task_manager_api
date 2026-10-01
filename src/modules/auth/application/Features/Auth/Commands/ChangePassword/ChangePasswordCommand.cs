using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.ChangePassword;

/// <summary>
/// লগইন থাকা অবস্থায় পাসওয়ার্ড পরিবর্তন করার কমান্ড:
/// </summary>
public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : ICommand<bool>;
