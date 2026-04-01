using Users.Domain;
using System.Collections.Concurrent;

namespace Users.Data;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, UserModel> _store = new();

    public InMemoryUserRepository()
    {
        Seed(new UserModel { Name = "Alice", Email = "alice@example.com" });
        Seed(new UserModel { Name = "Bob", Email = "bob@example.com" });
    }

    private void Seed(UserModel user)
    {
        _store[user.Id] = user;
    }

    public Task<UserModel> CreateAsync(UserModel user)
    {
        var item = user with { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        _store[item.Id] = item;
        return Task.FromResult(item);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var removed = _store.TryRemove(id, out _);
        return Task.FromResult(removed);
    }

    public Task<UserModel?> GetAsync(Guid id)
    {
        _store.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<IEnumerable<UserModel>> ListAsync() =>
        Task.FromResult(_store.Values.OrderBy(x => x.CreatedAt).AsEnumerable());

    public Task<UserModel?> UpdateAsync(Guid id, UserModel user)
    {
        if (!_store.TryGetValue(id, out var existing))
            return Task.FromResult<UserModel?>(null);

        var updated = existing with { Name = user.Name, Email = user.Email };
        var replaced = _store.TryUpdate(id, updated, existing);
        return Task.FromResult<UserModel?>(replaced ? updated : null);
    }
}
