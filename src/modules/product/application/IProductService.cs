using App.Features.Product.Domain;
using Shared;

namespace App.Features.Product.Application;

public interface IProductService
{
    Task<Result<IEnumerable<ProductModel>>> GetProductsAsync();
    Task<Result<ProductModel>> GetProductAsync(Guid id);
    Task<Result<ProductModel>> CreateProductAsync(ProductModel product);
    Task<Result<ProductModel>> UpdateProductAsync(Guid id, ProductModel product);
    Task<Result<bool>> DeleteProductAsync(Guid id);
}
