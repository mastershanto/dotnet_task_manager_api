using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Users.Domain;

namespace Auth.Application.Features.Auth.Commands.Register;

/// <summary>
/// রেজিস্ট্রেশন হ্যান্ডলার (CQRS Command Handler):
/// ১. ইমেইল ডুপ্লিকেট কিনা তা পরীক্ষা করে।
/// ২. পাসওয়ার্ড নিরাপদে হ্যাশ করে।
/// ৩. প্রাথমিক আন-ভেরিফাইড ইউজার তৈরি করে।
/// ৪. ৬-সংখ্যার ওটিপি জেনারেট করে ডাটাবেসে সেভ করে।
/// ৫. ইউজারের অনুরোধ অনুসারে রেসপন্সে ওটিপি রিটার্ন করে (ভবিষ্যতে এসএমটিপি যুক্ত না হওয়া পর্যন্ত)।
/// </summary>
public class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegisterResponse>
{
    private readonly IAuthRepository _authRepository;
    private readonly IPasswordHasherService _hasher;
    private readonly IOtpService _otpService;

    public RegisterCommandHandler(
        IAuthRepository authRepository,
        IPasswordHasherService hasher,
        IOtpService otpService)
    {
        _authRepository = authRepository;
        _hasher = hasher;
        _otpService = otpService;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _authRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null && existingUser.IsEmailVerified)
        {
            return Result<RegisterResponse>.Failure("A verified user with this email already exists. Please login.");
        }

        UserModel user;
        if (existingUser is null)
        {
            var hashedPassword = _hasher.HashPassword(request.Password);
            var newUser = new UserModel
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Email = request.Email.Trim().ToLowerInvariant(),
                PasswordHash = hashedPassword,
                Role = "user",
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            user = await _authRepository.CreateUserAsync(newUser, cancellationToken);
        }
        else
        {
            // পূর্বে সাইন-আপ করেছিল কিন্তু ওটিপি ভেরিফাই করেনি
            var updatedUser = existingUser with
            {
                Name = request.Name.Trim(),
                PasswordHash = _hasher.HashPassword(request.Password),
                UpdatedAt = DateTime.UtcNow
            };
            user = await _authRepository.UpdateUserAsync(updatedUser, cancellationToken);
        }

        // পূর্বের সক্রিয় ওটিপি বাতিল করা
        await _authRepository.InvalidatePreviousOtpsAsync(user.Email, OtpPurpose.Registration, cancellationToken);

        // নতুন ৬-সংখ্যার ওটিপি জেনারেট করা (মেয়াদ: ১০ মিনিট)
        var otpCode = _otpService.GenerateOtp();
        var otpEntity = new OtpCodeModel
        {
            Id = Guid.NewGuid(),
            Email = user.Email,
            Code = otpCode,
            Purpose = OtpPurpose.Registration,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        await _authRepository.SaveOtpAsync(otpEntity, cancellationToken);

        var response = new RegisterResponse(
            Success: true,
            Message: "Registration initiated successfully. Please verify your OTP.",
            Email: user.Email,
            Otp: otpCode // টেস্টিং ও ইউজার নির্দেশ অনুযায়ী সাময়িকভাবে রেসপন্সে পাঠানো হচ্ছে
        );

        return Result<RegisterResponse>.Success(response);
    }
}
