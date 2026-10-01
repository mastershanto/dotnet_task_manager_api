using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.ChangePassword;

/// <summary>
/// পাসওয়ার্ড পরিবর্তন হ্যান্ডলার:
/// ১. ইউজারের বর্তমান পাসওয়ার্ড মিলিয়ে দেখে।
/// ২. সঠিক হলে নতুন পাসওয়ার্ড হ্যাশ করে ডাটাবেসে সেভ করে।
/// </summary>
public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, bool>
{
    private readonly IAuthRepository _authRepository;
    private readonly IPasswordHasherService _hasher;

    public ChangePasswordCommandHandler(
        IAuthRepository authRepository,
        IPasswordHasherService hasher)
    {
        _authRepository = authRepository;
        _hasher = hasher;
    }

    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<bool>.Failure("User account not found.");
        }

        var isCurrentPasswordValid = _hasher.VerifyPassword(request.CurrentPassword, user.PasswordHash);
        if (!isCurrentPasswordValid)
        {
            return Result<bool>.Failure("Current password is incorrect.");
        }

        var newHashedPassword = _hasher.HashPassword(request.NewPassword);
        var updatedUser = user with
        {
            PasswordHash = newHashedPassword,
            UpdatedAt = DateTime.UtcNow
        };

        await _authRepository.UpdateUserAsync(updatedUser, cancellationToken);

        return Result<bool>.Success(true);
    }
}
