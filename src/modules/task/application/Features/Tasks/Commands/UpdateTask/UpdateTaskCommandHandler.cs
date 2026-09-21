using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Tasks.Domain;

namespace Tasks.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHandler : ICommandHandler<UpdateTaskCommand, TaskItemModel>
{
    private readonly ITaskRepository _repository;

    public UpdateTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TaskItemModel>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return Result<TaskItemModel>.Failure($"Task with ID '{request.Id}' was not found.");
        }

        var taskToUpdate = existing with
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Status = request.Status,
            Priority = request.Priority,
            DueDate = request.DueDate,
            CategoryId = request.CategoryId,
            AssignedUserId = request.AssignedUserId,
            UpdatedAt = DateTime.UtcNow
        };

        var updated = await _repository.UpdateAsync(request.Id, taskToUpdate, cancellationToken);
        return updated is null 
            ? Result<TaskItemModel>.Failure("Failed to update task.") 
            : Result<TaskItemModel>.Success(updated);
    }
}
