using FluentValidation;

namespace TodoApi.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// Enterprise validation for CreateTask command
/// </summary>
public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required")
            .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters")
            .MinimumLength(3).WithMessage("Task title must be at least 3 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("Valid project ID is required");

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 3).WithMessage("Priority must be between 0 (Low) and 3 (Critical)");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Due date cannot be in the past")
            .When(x => x.DueDate.HasValue);

        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0).WithMessage("Estimated hours must be positive")
            .LessThanOrEqualTo(1000).WithMessage("Estimated hours cannot exceed 1000")
            .When(x => x.EstimatedHours.HasValue);
    }
}
