using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TodoApi.Application.Abstractions;
using TodoApi.Data;
using TodoApi.Domain.Common;

namespace TodoApi.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation - manages transactions and domain events
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly TodoContext _context;
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(TodoContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events before saving
        await DispatchDomainEventsAsync(cancellationToken);

        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await (_currentTransaction?.CommitAsync(cancellationToken) ?? Task.CompletedTask);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await (_currentTransaction?.RollbackAsync(cancellationToken) ?? Task.CompletedTask);
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = _context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        // Domain events would be published here via MediatR
        // This allows for eventual consistency and decoupled architecture
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
    }
}
