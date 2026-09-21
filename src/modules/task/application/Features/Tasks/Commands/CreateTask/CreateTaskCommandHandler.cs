using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Tasks.Domain;

namespace Tasks.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// CreateTaskCommand এক্সিকিউশনের হ্যান্ডলার:
/// শুধুমাত্র এই নির্দিষ্ট কমান্ড এক্সিকিউট করার জন্য দায়ী (Single Responsibility Principle)।
/// </summary>
public class CreateTaskCommandHandler : ICommandHandler<CreateTaskCommand, TaskItemModel>
{
    private readonly ITaskRepository _repository;

    public CreateTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TaskItemModel>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new TaskItemModel
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Status = request.Status,
            Priority = request.Priority,
            DueDate = request.DueDate,
            CategoryId = request.CategoryId,
            AssignedUserId = request.AssignedUserId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(task, cancellationToken);
        return Result<TaskItemModel>.Success(created);
    }
}
