using Auth.Domain;
using BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Domain;

namespace Auth.Data;

/// <summary>
/// Entity Framework Core Auth Repository (Infrastructure Data Access):
/// AppDbContext ব্যবহার করে ইউজার ও ওটিপি সংক্রান্ত ডাটাবেস অপারেশন পরিচালনা করে।
/// </summary>
public class EfAuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public EfAuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserModel?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalized, cancellationToken);
    }

    public async Task<UserModel?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<UserModel> CreateUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        var item = user with
        {
            Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id,
            CreatedAt = user.CreatedAt == default ? DateTime.UtcNow : user.CreatedAt
        };

        await _context.Users.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<UserModel> UpdateUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Users.FindAsync(new object[] { user.Id }, cancellationToken);
        if (existing is not null)
        {
            _context.Entry(existing).State = EntityState.Detached;
        }

        var updated = user with { UpdatedAt = DateTime.UtcNow };
        _context.Users.Update(updated);
        await _context.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        _context.Users.Remove(existing);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task SaveOtpAsync(OtpCodeModel otp, CancellationToken cancellationToken = default)
    {
        await _context.OtpCodes.AddAsync(otp, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<OtpCodeModel?> GetActiveOtpAsync(
        string email,
        string code,
        OtpPurpose purpose,
        CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var now = DateTime.UtcNow;

        return await _context.OtpCodes
            .Where(o => o.Email.ToLower() == normalized &&
                        o.Code == code.Trim() &&
                        o.Purpose == purpose &&
                        !o.IsUsed &&
                        o.ExpiresAt > now)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task MarkOtpUsedAsync(Guid otpId, CancellationToken cancellationToken = default)
    {
        var otp = await _context.OtpCodes.FindAsync(new object[] { otpId }, cancellationToken);
        if (otp is not null)
        {
            _context.Entry(otp).State = EntityState.Detached;
            var updated = otp with { IsUsed = true };
            _context.OtpCodes.Update(updated);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task InvalidatePreviousOtpsAsync(
        string email,
        OtpPurpose purpose,
        CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var activeOtps = await _context.OtpCodes
            .Where(o => o.Email.ToLower() == normalized && o.Purpose == purpose && !o.IsUsed)
            .ToListAsync(cancellationToken);

        foreach (var otp in activeOtps)
        {
            _context.Entry(otp).State = EntityState.Detached;
            _context.OtpCodes.Update(otp with { IsUsed = true });
        }

        if (activeOtps.Count > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
