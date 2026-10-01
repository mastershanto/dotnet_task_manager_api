using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.VerifyResetOtp;

/// <summary>
/// পাসওয়ার্ড রিসেট ওটিপি ভ্যালিডেশন হ্যান্ডলার:
/// ওটিপি সঠিক ও ভ্যালিড থাকলে Success(true) প্রদান করে।
/// </summary>
public class VerifyResetOtpCommandHandler : ICommandHandler<VerifyResetOtpCommand, bool>
{
    private readonly IAuthRepository _authRepository;

    public VerifyResetOtpCommandHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<Result<bool>> Handle(VerifyResetOtpCommand request, CancellationToken cancellationToken)
    {
        var activeOtp = await _authRepository.GetActiveOtpAsync(
            request.Email,
            request.Otp,
            OtpPurpose.PasswordReset,
            cancellationToken);

        if (activeOtp is null)
        {
            return Result<bool>.Failure("Invalid or expired OTP code.");
        }

        return Result<bool>.Success(true);
    }
}
