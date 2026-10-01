using Auth.Domain;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.UpdateProfile;

/// <summary>
/// প্রোফাইল আপডেট করার কমান্ড:
/// </summary>
public record UpdateProfileCommand(
    Guid UserId,
    string Name
) : ICommand<UserProfileResponse>;
