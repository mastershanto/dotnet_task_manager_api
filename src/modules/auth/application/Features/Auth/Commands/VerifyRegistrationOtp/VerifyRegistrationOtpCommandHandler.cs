using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.VerifyRegistrationOtp;

/// <summary>
/// রেজিস্ট্রেশন ওটিপি যাচাইকরণ হ্যান্ডলার:
/// ১. ডাটাবেস থেকে সক্রিয় ওটিপি কোড চেক করে।
/// ২. ওটিপি সঠিক ও মেয়াদোত্তীর্ণ না হলে ওটিপি-কে 'Used' হিসেবে মার্ক করে।
/// ৩. ইউজারের IsEmailVerified ফ্ল্যাগ True করে।
/// ৪. স্বয়ংক্রিয়ভাবে JWT টোকেন ইস্যু করে সরাসরি লগইন করায়।
/// </summary>
public class VerifyRegistrationOtpCommandHandler : ICommandHandler<VerifyRegistrationOtpCommand, AuthResponse>
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;

    public VerifyRegistrationOtpCommandHandler(
        IAuthRepository authRepository,
        ITokenService tokenService)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(VerifyRegistrationOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return Result<AuthResponse>.Failure("User account not found.");
        }

        if (user.IsEmailVerified)
        {
            return Result<AuthResponse>.Failure("Email is already verified. Please login directly.");
        }

        var activeOtp = await _authRepository.GetActiveOtpAsync(
            request.Email,
            request.Otp,
            OtpPurpose.Registration,
            cancellationToken);

        if (activeOtp is null)
        {
            return Result<AuthResponse>.Failure("Invalid or expired OTP code. Please request a new one.");
        }

        // ওটিপি ব্যবহার সম্পন্ন করা
        await _authRepository.MarkOtpUsedAsync(activeOtp.Id, cancellationToken);

        // ইউজার ভেরিফিকেশন সক্রিয় করা
        var verifiedUser = user with { IsEmailVerified = true, UpdatedAt = DateTime.UtcNow };
        await _authRepository.UpdateUserAsync(verifiedUser, cancellationToken);

        // অ্যাক্সেস টোকেন তৈরি করা
        var token = _tokenService.GenerateToken(verifiedUser);

        var response = new AuthResponse(
            Success: true,
            Message: "Email verified successfully. Welcome!",
            Token: token,
            UserId: verifiedUser.Id,
            Name: verifiedUser.Name,
            Email: verifiedUser.Email,
            Role: verifiedUser.Role
        );

        return Result<AuthResponse>.Success(response);
    }
}
