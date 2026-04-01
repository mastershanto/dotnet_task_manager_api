using Products.Domain;
using System.Collections.Concurrent;

namespace Products.Data;

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, ProductModel> _store = new();

    public InMemoryProductRepository()
    {
        Seed(new ProductModel { Title = "Smartphone", Description = "4G smartphone", Price = 199.99m, Category = "Electronics" });
        Seed(new ProductModel { Title = "Backpack", Description = "Travel backpack", Price = 49.99m, Category = "Accessories" });
    }

    private void Seed(ProductModel product)
    {
        _store[product.Id] = product;
    }

    public Task<ProductModel> CreateAsync(ProductModel product)
    {
        var item = product with { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        _store[item.Id] = item;
        return Task.FromResult(item);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var removed = _store.TryRemove(id, out _);
        return Task.FromResult(removed);
    }

    public Task<ProductModel?> GetAsync(Guid id)
    {
        _store.TryGetValue(id, out var product);
        return Task.FromResult(product);
    }

    public Task<IEnumerable<ProductModel>> ListAsync() =>
        Task.FromResult(_store.Values.OrderBy(x => x.CreatedAt).AsEnumerable());

    public Task<ProductModel?> UpdateAsync(Guid id, ProductModel product)
    {
        if (!_store.TryGetValue(id, out var existing))
            return Task.FromResult<ProductModel?>(null);

        var updated = existing with
        {
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category
        };

        var replaced = _store.TryUpdate(id, updated, existing);
        return Task.FromResult<ProductModel?>(replaced ? updated : null);
    }
}