using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.ForgotPassword;

/// <summary>
/// ফরগট পাসওয়ার্ড হ্যান্ডলার:
/// ১. ইউজারের অস্তিত্ব যাচাই করে।
/// ২. পূর্ববর্তী পাসওয়ার্ড রিসেট ওটিপি বাতিল করে।
/// ৩. নতুন ৬-সংখ্যার ওটিপি কোড জেনারেট ও সংরক্ষণ করে।
/// ৪. রেসপন্সে ওটিপি রিটার্ন করে (ভবিষ্যতে ইমেইলে যাবে)।
/// </summary>
public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private readonly IAuthRepository _authRepository;
    private readonly IOtpService _otpService;

    public ForgotPasswordCommandHandler(
        IAuthRepository authRepository,
        IOtpService otpService)
    {
        _authRepository = authRepository;
        _otpService = otpService;
    }

    public async Task<Result<ForgotPasswordResponse>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return Result<ForgotPasswordResponse>.Failure("User account with this email was not found.");
        }

        // পূর্বের ওটিপিগুলো অকার্যকর করা
        await _authRepository.InvalidatePreviousOtpsAsync(user.Email, OtpPurpose.PasswordReset, cancellationToken);

        // নতুন রিসেট ওটিপি তৈরি করা (মেয়াদ: ১০ মিনিট)
        var otpCode = _otpService.GenerateOtp();
        var otpEntity = new OtpCodeModel
        {
            Id = Guid.NewGuid(),
            Email = user.Email,
            Code = otpCode,
            Purpose = OtpPurpose.PasswordReset,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        await _authRepository.SaveOtpAsync(otpEntity, cancellationToken);

        var response = new ForgotPasswordResponse(
            Success: true,
            Message: "Password reset OTP has been sent.",
            Email: user.Email,
            Otp: otpCode
        );

        return Result<ForgotPasswordResponse>.Success(response);
    }
}
