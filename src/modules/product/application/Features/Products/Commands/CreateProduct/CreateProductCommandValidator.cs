using FluentValidation;

namespace Products.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Product title is required.")
            .MinimumLength(2).WithMessage("Product title must be at least 2 characters.")
            .MaximumLength(150).WithMessage("Product title cannot exceed 150 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product description is required.")
            .MaximumLength(500).WithMessage("Product description cannot exceed 500 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Product price must be greater than zero.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Product category is required.");
    }
}
