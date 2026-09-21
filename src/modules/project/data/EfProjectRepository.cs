using BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;
using Projects.Domain;

namespace Projects.Data;

/// <summary>
/// Entity Framework Core Project Repository:
/// </summary>
public class EfProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public EfProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectModel>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProjectModel?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ProjectModel> CreateAsync(ProjectModel project, CancellationToken cancellationToken = default)
    {
        var item = project with
        {
            Id = project.Id == Guid.Empty ? Guid.NewGuid() : project.Id,
            CreatedAt = project.CreatedAt == default ? DateTime.UtcNow : project.CreatedAt
        };

        await _context.Projects.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<ProjectModel?> UpdateAsync(Guid id, ProjectModel project, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Projects.FindAsync(new object[] { id }, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        _context.Entry(existing).State = EntityState.Detached;

        var updated = existing with
        {
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            Color = project.Color,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            OwnerId = project.OwnerId,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Projects.Update(updated);
        await _context.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Projects.FindAsync(new object[] { id }, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        _context.Projects.Remove(existing);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
