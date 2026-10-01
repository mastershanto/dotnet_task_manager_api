using FluentValidation;

namespace Auth.Application.Features.Auth.Commands.DeleteAccount;

public class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
{
    public DeleteAccountCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password confirmation is required to delete the account.");
    }
}
