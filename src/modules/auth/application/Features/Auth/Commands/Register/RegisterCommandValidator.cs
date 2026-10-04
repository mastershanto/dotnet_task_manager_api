using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("User type is required.")
            .Must(t => t is "student" or "instructor" or "renter")
            .WithMessage("Type must be student, instructor, or renter.");

        RuleFor(x => x.AgreeToTerms)
            .Equal(true).WithMessage("You must agree to the terms.");
    }
}
