using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.UpdateProfile;

/// <summary>
/// প্রোফাইল এডিট হ্যান্ডলার:
/// ইউজারের নাম ইত্যাদি আপডেট করে।
/// </summary>
public class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, UserProfileResponse>
{
    private readonly IAuthRepository _authRepository;

    public UpdateProfileCommandHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<Result<UserProfileResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<UserProfileResponse>.Failure("User profile not found.");
        }

        var updatedUser = user with
        {
            Name = request.Name.Trim(),
            UpdatedAt = DateTime.UtcNow
        };

        var saved = await _authRepository.UpdateUserAsync(updatedUser, cancellationToken);

        var profile = new UserProfileResponse(
            Id: saved.Id,
            Name: saved.Name,
            Email: saved.Email,
            Role: saved.Role,
            IsEmailVerified: saved.IsEmailVerified,
            CreatedAt: saved.CreatedAt,
            UpdatedAt: saved.UpdatedAt
        );

        return Result<UserProfileResponse>.Success(profile);
    }
}
