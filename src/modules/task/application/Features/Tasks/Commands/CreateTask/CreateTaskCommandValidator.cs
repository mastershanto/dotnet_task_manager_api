using FluentValidation;

namespace Tasks.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// FluentValidation রুলস (Pipeline Behavior দ্বারা স্বয়ংক্রিয়ভাবে এক্সিকিউট হবে):
/// </summary>
public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MinimumLength(3).WithMessage("Task title must be at least 3 characters.")
            .MaximumLength(150).WithMessage("Task title cannot exceed 150 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.DueDate)
            .Must(date => !date.HasValue || date.Value >= DateTime.UtcNow.Date.AddDays(-1))
            .WithMessage("Due date cannot be in the past.");
    }
}
