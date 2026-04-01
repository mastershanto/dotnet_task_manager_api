namespace Products.Domain;

public interface IProductRepository
{
    Task<IEnumerable<ProductModel>> ListAsync();
    Task<ProductModel?> GetAsync(Guid id);
    Task<ProductModel> CreateAsync(ProductModel product);
    Task<ProductModel?> UpdateAsync(Guid id, ProductModel product);
    Task<bool> DeleteAsync(Guid id);
}
