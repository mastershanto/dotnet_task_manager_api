using FluentValidation;
using TodoApi.DTOs;

namespace TodoApi.Validation;

public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.ProjectId)
            .GreaterThan(0);

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 4);

        RuleFor(x => x.Tags)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Tags));

        RuleFor(x => x.EstimatedHours)
            .GreaterThanOrEqualTo(0)
            .When(x => x.EstimatedHours.HasValue);
    }
}

public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
{
    public UpdateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 4)
            .When(x => x.Priority.HasValue);

        RuleFor(x => x.Progress)
            .InclusiveBetween(0, 100)
            .When(x => x.Progress.HasValue);

        RuleFor(x => x.Tags)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Tags));

        RuleFor(x => x.EstimatedHours)
            .GreaterThanOrEqualTo(0)
            .When(x => x.EstimatedHours.HasValue);

        RuleFor(x => x.ActualHours)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ActualHours.HasValue);
    }
}
