using Auth.Domain;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Queries.GetProfile;

/// <summary>
/// বর্তমান ইউজারের প্রোফাইল দেখার কুয়েরি (CQRS Query):
/// </summary>
public record GetProfileQuery(
    Guid UserId
) : IQuery<UserProfileResponse>;
