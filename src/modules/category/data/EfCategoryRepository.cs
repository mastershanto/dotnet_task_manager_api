using BuildingBlocks.Persistence;
using Categories.Domain;
using Microsoft.EntityFrameworkCore;

namespace Categories.Data;

public class EfCategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public EfCategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryModel>> ListAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<CategoryModel?> GetAsync(Guid id)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<CategoryModel> CreateAsync(CategoryModel category)
    {
        var item = category with
        {
            Id = category.Id == Guid.Empty ? Guid.NewGuid() : category.Id,
            CreatedAt = category.CreatedAt == default ? DateTime.UtcNow : category.CreatedAt
        };

        await _context.Categories.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<CategoryModel?> UpdateAsync(Guid id, CategoryModel category)
    {
        var existing = await _context.Categories.FindAsync(id);
        if (existing is null)
        {
            return null;
        }

        _context.Entry(existing).State = EntityState.Detached;

        var updated = existing with
        {
            Name = category.Name,
            Description = category.Description,
            Color = category.Color,
            Icon = category.Icon
        };

        _context.Categories.Update(updated);
        await _context.SaveChangesAsync();
        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Categories.FindAsync(id);
        if (existing is null)
        {
            return false;
        }

        _context.Categories.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
