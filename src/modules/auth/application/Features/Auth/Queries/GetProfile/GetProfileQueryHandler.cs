using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Queries.GetProfile;

/// <summary>
/// প্রোফাইল রিড হ্যান্ডলার (Query Side):
/// ডাটাবেস থেকে ইউজারের তথ্য নিয়ে আসে (পাসওয়ার্ড ফিল্ড বাদ দিয়ে নিরাপদ DTO প্রদান করে)।
/// </summary>
public class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, UserProfileResponse>
{
    private readonly IAuthRepository _authRepository;

    public GetProfileQueryHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<Result<UserProfileResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<UserProfileResponse>.Failure("User profile not found.");
        }

        var profile = new UserProfileResponse(
            Id: user.Id,
            Name: user.Name,
            Email: user.Email,
            Role: user.Role,
            IsEmailVerified: user.IsEmailVerified,
            CreatedAt: user.CreatedAt,
            UpdatedAt: user.UpdatedAt
        );

        return Result<UserProfileResponse>.Success(profile);
    }
}
