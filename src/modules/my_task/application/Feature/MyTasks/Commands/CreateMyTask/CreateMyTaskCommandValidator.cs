
using FluentValidation;

namespace Tasks.Application.Features.MyTasks.Commands.CreateMyTask;

public class CreateMyTaskCommandValidator: AbstractValidator<CreateMyTaskCommand>
{
   public CreateMyTaskCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty()
        .WithMessage("Task title is required")
        .MinimumLength(3).WriteMessage("My Task title must be at least 3 characters.")
        .MaximumLength(150).WithMessage("My Task title cannot exceed 150 charecters.");

        RoleFor(x => x.Description)
        .MaximumLength(1000).WithMessage("Description must be under the 1000 words");

        RoleFor(x => x.DueDate).Must(date => 
        !date.HasValue||date.Value 
        >=DateTime.UtcNow.Date.AddDays(-1))
        .WithMessage("Due date cannot be in the past");


    }

}
