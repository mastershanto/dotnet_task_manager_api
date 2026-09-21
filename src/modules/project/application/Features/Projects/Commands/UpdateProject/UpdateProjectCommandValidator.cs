using FluentValidation;

namespace Projects.Application.Features.Projects.Commands.UpdateProject;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Project ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required.")
            .MinimumLength(2).WithMessage("Project name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Project name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Project description cannot exceed 500 characters.");

        RuleFor(x => x)
            .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.EndDate >= x.StartDate)
            .WithMessage("End date must be greater than or equal to start date.");
    }
}
