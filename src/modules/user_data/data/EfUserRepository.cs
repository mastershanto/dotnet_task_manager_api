using BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Domain;

namespace Users.Data;

public class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public EfUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserModel>> ListAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<UserModel?> GetAsync(Guid id)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<UserModel> CreateAsync(UserModel user)
    {
        var item = user with
        {
            Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id,
            CreatedAt = user.CreatedAt == default ? DateTime.UtcNow : user.CreatedAt
        };

        await _context.Users.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<UserModel?> UpdateAsync(Guid id, UserModel user)
    {
        var existing = await _context.Users.FindAsync(id);
        if (existing is null)
        {
            return null;
        }

        _context.Entry(existing).State = EntityState.Detached;

        var updated = existing with
        {
            Name = user.Name,
            Email = user.Email
        };

        _context.Users.Update(updated);
        await _context.SaveChangesAsync();
        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Users.FindAsync(id);
        if (existing is null)
        {
            return false;
        }

        _context.Users.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
