using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.Register;

/// <summary>
/// রেজিস্ট্রেশন ইনপুট ভ্যালিডেশন রুলস (FluentValidation):
/// জুনিয়র ডেভেলপারদের সহজে বোঝার সুবিধার্থে প্রতিটি ফিল্ডের জন্য স্পষ্ট বার্তা দেওয়া হলো।
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("User name is required.")
            .MinimumLength(2).WithMessage("User name must be at least 2 characters long.")
            .MaximumLength(100).WithMessage("User name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Please provide a valid email address.")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}
