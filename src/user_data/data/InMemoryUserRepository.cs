using App.Features.User.Domain;

namespace App.Features.User.Data;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<UserModel> _store = new()
    {
        new UserModel { Name = "Alice", Email = "alice@example.com" },
        new UserModel { Name = "Bob", Email = "bob@example.com" }
    };

    public Task<UserModel> CreateAsync(UserModel user)
    {
        var item = user with { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        _store.Add(item);
        return Task.FromResult(item);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var found = _store.FirstOrDefault(x => x.Id == id);
        if (found is null) return Task.FromResult(false);

        _store.Remove(found);
        return Task.FromResult(true);
    }

    public Task<UserModel?> GetAsync(Guid id)
    {
        return Task.FromResult(_store.FirstOrDefault(x => x.Id == id));
    }

    public Task<IEnumerable<UserModel>> ListAsync() => Task.FromResult(_store.AsEnumerable());

    public Task<UserModel?> UpdateAsync(Guid id, UserModel user)
    {
        var found = _store.FirstOrDefault(x => x.Id == id);
        if (found is null) return Task.FromResult<UserModel?>(null);

        var updated = found with { Name = user.Name, Email = user.Email };
        _store.Remove(found);
        _store.Add(updated);
        return Task.FromResult<UserModel?>(updated);
    }
}
