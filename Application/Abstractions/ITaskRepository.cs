using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Application.Abstractions;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdWithDetailsAsync(int taskId);
    Task<TaskItem?> GetByIdAsync(int taskId);

    Task<(List<TaskItem> Items, int TotalCount)> GetProjectTasksAsync(int projectId, QueryParams queryParams);
    Task<List<TaskItem>> GetUserTasksAsync(int userId, QueryParams queryParams);

    Task AddAsync(TaskItem task);
    Task SaveChangesAsync();
}
