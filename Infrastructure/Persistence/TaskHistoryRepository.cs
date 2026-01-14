using TodoApi.Application.Abstractions;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Infrastructure.Persistence;

public sealed class TaskHistoryRepository : ITaskHistoryRepository
{
    private readonly TodoContext _context;

    public TaskHistoryRepository(TodoContext context)
    {
        _context = context;
    }

    public Task AddAsync(TaskHistory history)
    {
        _context.TaskHistory.Add(history);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
