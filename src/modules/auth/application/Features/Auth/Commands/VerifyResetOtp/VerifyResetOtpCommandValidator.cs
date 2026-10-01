using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.VerifyResetOtp;

public class VerifyResetOtpCommandValidator : AbstractValidator<VerifyResetOtpCommand>
{
    public VerifyResetOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.");

        RuleFor(x => x.Otp)
            .NotEmpty().WithMessage("OTP is required.")
            .Length(6).WithMessage("OTP must be exactly 6 digits.");
    }
}
