using TodoApi.Models;

namespace TodoApi.Application.Abstractions;

public interface ITaskHistoryRepository
{
    Task AddAsync(TaskHistory history);
    Task SaveChangesAsync();
}
