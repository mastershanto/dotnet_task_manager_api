using FluentValidation;
using TodoApi.DTOs;

namespace TodoApi.Validation;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(200);

        RuleFor(x => x.FullName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.FullName));
    }
}

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(200);
    }
}
