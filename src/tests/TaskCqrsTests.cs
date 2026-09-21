using BuildingBlocks.Abstractions;
using Tasks.Application.Features.Tasks.Commands.CreateTask;
using Tasks.Application.Features.Tasks.Commands.DeleteTask;
using Tasks.Application.Features.Tasks.Commands.UpdateTask;
using Tasks.Application.Features.Tasks.Queries.GetTaskById;
using Tasks.Application.Features.Tasks.Queries.GetTasks;
using Tasks.Domain;

namespace Api.Tests;

public class FakeTaskRepository : ITaskRepository
{
    private readonly List<TaskItemModel> _tasks = new();

    public Task<IEnumerable<TaskItemModel>> ListAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IEnumerable<TaskItemModel>>(_tasks.ToList());

    public Task<TaskItemModel?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id));

    public Task<TaskItemModel> CreateAsync(TaskItemModel task, CancellationToken cancellationToken = default)
    {
        _tasks.Add(task);
        return Task.FromResult(task);
    }

    public Task<TaskItemModel?> UpdateAsync(Guid id, TaskItemModel task, CancellationToken cancellationToken = default)
    {
        var idx = _tasks.FindIndex(t => t.Id == id);
        if (idx == -1) return Task.FromResult<TaskItemModel?>(null);
        _tasks[idx] = task;
        return Task.FromResult<TaskItemModel?>(task);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = _tasks.FirstOrDefault(t => t.Id == id);
        if (item is null) return Task.FromResult(false);
        _tasks.Remove(item);
        return Task.FromResult(true);
    }
}

public class TaskCqrsTests
{
    [Fact]
    public async Task CreateTaskCommand_Success_WhenValid()
    {
        var repo = new FakeTaskRepository();
        var handler = new CreateTaskCommandHandler(repo);

        var command = new CreateTaskCommand("Build CQRS Architecture", "World best pattern", TaskItemStatus.InProgress, TaskPriority.High);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Build CQRS Architecture", result.Value.Title);
        Assert.Equal(TaskItemStatus.InProgress, result.Value.Status);
    }

    [Fact]
    public void CreateTaskValidator_Fails_WhenTitleIsTooShort()
    {
        var validator = new CreateTaskCommandValidator();
        var command = new CreateTaskCommand("AB", "Short title");

        var validationResult = validator.Validate(command);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(CreateTaskCommand.Title));
    }

    [Fact]
    public async Task GetTasksQuery_FiltersByStatus()
    {
        var repo = new FakeTaskRepository();
        await repo.CreateAsync(new TaskItemModel { Title = "Task 1", Status = TaskItemStatus.Todo });
        await repo.CreateAsync(new TaskItemModel { Title = "Task 2", Status = TaskItemStatus.Completed });

        var handler = new GetTasksQueryHandler(repo);
        var result = await handler.Handle(new GetTasksQuery(Status: TaskItemStatus.Completed), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
        Assert.Equal("Task 2", result.Value!.First().Title);
    }

    [Fact]
    public async Task UpdateTaskCommand_ReturnsFailure_WhenNotFound()
    {
        var repo = new FakeTaskRepository();
        var handler = new UpdateTaskCommandHandler(repo);

        var command = new UpdateTaskCommand(Guid.NewGuid(), "Updated Title", "Desc", TaskItemStatus.Completed, TaskPriority.High, null, null, null);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Errors.First(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteTaskCommand_ReturnsSuccess_WhenExists()
    {
        var repo = new FakeTaskRepository();
        var existing = await repo.CreateAsync(new TaskItemModel { Title = "To be deleted" });
        var handler = new DeleteTaskCommandHandler(repo);

        var result = await handler.Handle(new DeleteTaskCommand(existing.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }
}
