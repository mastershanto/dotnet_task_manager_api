using Microsoft.EntityFrameworkCore;
using TodoApi.Application.Abstractions;
using TodoApi.Data;
using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Infrastructure.Persistence;

public sealed class TaskRepository : ITaskRepository
{
    private readonly TodoContext _context;

    public TaskRepository(TodoContext context)
    {
        _context = context;
    }

    public Task<TaskItem?> GetByIdWithDetailsAsync(int taskId)
    {
        return _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .Include(t => t.CreatedBy)
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == taskId && !t.DeletedAt.HasValue);
    }

    public Task<TaskItem?> GetByIdAsync(int taskId)
    {
        return _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && !t.DeletedAt.HasValue);
    }

    public async Task<(List<TaskItem> Items, int TotalCount)> GetProjectTasksAsync(int projectId, QueryParams queryParams)
    {
        var query = _context.Tasks
            .Where(t => t.ProjectId == projectId && !t.DeletedAt.HasValue)
            .Include(t => t.Assignee)
            .Include(t => t.CreatedBy)
            .AsQueryable();

        if (queryParams.Status.HasValue)
            query = query.Where(t => (int)t.Status == queryParams.Status);

        if (queryParams.Priority.HasValue)
            query = query.Where(t => (int)t.Priority == queryParams.Priority);

        if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            query = query.Where(t => t.Title.Contains(queryParams.SearchTerm) ||
                                     (t.Description != null && t.Description.Contains(queryParams.SearchTerm)));

        query = queryParams.SortBy?.ToLowerInvariant() switch
        {
            "priority" => queryParams.SortDescending
                ? query.OrderByDescending(t => t.Priority)
                : query.OrderBy(t => t.Priority),
            "duedate" => queryParams.SortDescending
                ? query.OrderByDescending(t => t.DueDate)
                : query.OrderBy(t => t.DueDate),
            "assignee" => queryParams.SortDescending
                ? query.OrderByDescending(t => t.Assignee != null ? t.Assignee.Username : string.Empty)
                : query.OrderBy(t => t.Assignee != null ? t.Assignee.Username : string.Empty),
            _ => queryParams.SortDescending
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<TaskItem>> GetUserTasksAsync(int userId, QueryParams queryParams)
    {
        var query = _context.Tasks
            .Where(t => t.AssigneeId == userId && !t.DeletedAt.HasValue)
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .Include(t => t.CreatedBy)
            .OrderByDescending(t => t.CreatedAt)
            .AsQueryable();

        return await query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();
    }

    public Task AddAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
