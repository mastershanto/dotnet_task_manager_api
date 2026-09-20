using BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;
using Products.Domain;

namespace Products.Data;

public class EfProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public EfProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductModel>> ListAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<ProductModel?> GetAsync(Guid id)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ProductModel> CreateAsync(ProductModel product)
    {
        var item = product with
        {
            Id = product.Id == Guid.Empty ? Guid.NewGuid() : product.Id,
            CreatedAt = product.CreatedAt == default ? DateTime.UtcNow : product.CreatedAt
        };

        await _context.Products.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<ProductModel?> UpdateAsync(Guid id, ProductModel product)
    {
        var existing = await _context.Products.FindAsync(id);
        if (existing is null)
        {
            return null;
        }

        _context.Entry(existing).State = EntityState.Detached;

        var updated = existing with
        {
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category
        };

        _context.Products.Update(updated);
        await _context.SaveChangesAsync();
        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Products.FindAsync(id);
        if (existing is null)
        {
            return false;
        }

        _context.Products.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
