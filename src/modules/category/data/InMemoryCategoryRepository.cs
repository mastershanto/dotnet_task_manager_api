using Categories.Domain;
using System.Collections.Concurrent;

namespace Categories.Data;

public class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly ConcurrentDictionary<Guid, CategoryModel> _store = new();

    public InMemoryCategoryRepository()
    {
        Seed(new CategoryModel
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
            Name = "Work",
            Description = "Work and professional tasks",
            Color = "#3B82F6",
            Icon = "briefcase"
        });
        Seed(new CategoryModel
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
            Name = "Personal",
            Description = "Personal errands and daily routine",
            Color = "#10B981",
            Icon = "user"
        });
    }

    private void Seed(CategoryModel category)
    {
        _store[category.Id] = category;
    }

    public Task<CategoryModel> CreateAsync(CategoryModel category)
    {
        var item = category with { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        _store[item.Id] = item;
        return Task.FromResult(item);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var removed = _store.TryRemove(id, out _);
        return Task.FromResult(removed);
    }

    public Task<CategoryModel?> GetAsync(Guid id)
    {
        _store.TryGetValue(id, out var category);
        return Task.FromResult(category);
    }

    public Task<IEnumerable<CategoryModel>> ListAsync() =>
        Task.FromResult(_store.Values.OrderBy(x => x.CreatedAt).AsEnumerable());

    public Task<CategoryModel?> UpdateAsync(Guid id, CategoryModel category)
    {
        if (!_store.TryGetValue(id, out var existing))
            return Task.FromResult<CategoryModel?>(null);

        var updated = existing with
        {
            Name = category.Name,
            Description = category.Description,
            Color = category.Color,
            Icon = category.Icon
        };

        var replaced = _store.TryUpdate(id, updated, existing);
        return Task.FromResult<CategoryModel?>(replaced ? updated : null);
    }
}
