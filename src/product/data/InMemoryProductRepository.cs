using App.Features.Product.Domain;

namespace App.Features.Product.Data;

public class InMemoryProductRepository : IProductRepository
{
    private readonly List<ProductModel> _store = new()
    {
        new ProductModel { Title = "Smartphone", Description = "4G smartphone", Price = 199.99m, Category = "Electronics" },
        new ProductModel { Title = "Backpack", Description = "Travel backpack", Price = 49.99m, Category = "Accessories" }
    };

    public Task<ProductModel> CreateAsync(ProductModel product)
    {
        var item = product with { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
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

    public Task<ProductModel?> GetAsync(Guid id)
    {
        return Task.FromResult(_store.FirstOrDefault(x => x.Id == id));
    }

    public Task<IEnumerable<ProductModel>> ListAsync() => Task.FromResult(_store.AsEnumerable());

    public Task<ProductModel?> UpdateAsync(Guid id, ProductModel product)
    {
        var found = _store.FirstOrDefault(x => x.Id == id);
        if (found is null) return Task.FromResult<ProductModel?>(null);

        var updated = found with
        {
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category
        };

        _store.Remove(found);
        _store.Add(updated);

        return Task.FromResult<ProductModel?>(updated);
    }
}