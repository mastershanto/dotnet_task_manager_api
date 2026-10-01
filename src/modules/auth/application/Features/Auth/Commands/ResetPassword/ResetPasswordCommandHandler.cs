using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.ResetPassword;

/// <summary>
/// পাসওয়ার্ড রিসেট হ্যান্ডলার:
/// ১. ওটিপির সত্যতা ও মেয়াদ যাচাই করে।
/// ২. ওটিপি ব্যবহৃত হিসেবে চিহ্নিত করে।
/// ৩. নতুন পাসওয়ার্ড সিকিউরলি হ্যাশ করে ইউজারের তথ্য আপডেট করে।
/// </summary>
public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, bool>
{
    private readonly IAuthRepository _authRepository;
    private readonly IPasswordHasherService _hasher;

    public ResetPasswordCommandHandler(
        IAuthRepository authRepository,
        IPasswordHasherService hasher)
    {
        _authRepository = authRepository;
        _hasher = hasher;
    }

    public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return Result<bool>.Failure("User account not found.");
        }

        var activeOtp = await _authRepository.GetActiveOtpAsync(
            request.Email,
            request.Otp,
            OtpPurpose.PasswordReset,
            cancellationToken);

        if (activeOtp is null)
        {
            return Result<bool>.Failure("Invalid or expired OTP code.");
        }

        // ওটিপি বন্ধ করা
        await _authRepository.MarkOtpUsedAsync(activeOtp.Id, cancellationToken);

        // নতুন পাসওয়ার্ড হ্যাশ করে আপডেট করা
        var newHashedPassword = _hasher.HashPassword(request.NewPassword);
        var updatedUser = user with
        {
            PasswordHash = newHashedPassword,
            IsEmailVerified = true, // ইমেইল ওটিপি প্রমাণ করায় একাউন্ট ভেরিফাইড
            UpdatedAt = DateTime.UtcNow
        };

        await _authRepository.UpdateUserAsync(updatedUser, cancellationToken);

        return Result<bool>.Success(true);
    }
}
