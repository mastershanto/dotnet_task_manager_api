using BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;
using Tasks.Domain;

namespace Tasks.Data;

/// <summary>
/// Entity Framework Core Task Repository (Infrastructure Data Access):
/// </summary>
public class EfTaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public EfTaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskItemModel>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItemModel?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TaskItemModel> CreateAsync(TaskItemModel task, CancellationToken cancellationToken = default)
    {
        var item = task with
        {
            Id = task.Id == Guid.Empty ? Guid.NewGuid() : task.Id,
            CreatedAt = task.CreatedAt == default ? DateTime.UtcNow : task.CreatedAt
        };

        await _context.Tasks.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<TaskItemModel?> UpdateAsync(Guid id, TaskItemModel task, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Tasks.FindAsync(new object[] { id }, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        _context.Entry(existing).State = EntityState.Detached;

        var updated = existing with
        {
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            CategoryId = task.CategoryId,
            AssignedUserId = task.AssignedUserId,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Update(updated);
        await _context.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Tasks.FindAsync(new object[] { id }, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        _context.Tasks.Remove(existing);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
