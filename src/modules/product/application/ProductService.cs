using App.Features.Product.Data;
using App.Features.Product.Domain;
using Shared;

namespace App.Features.Product.Application;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IEnumerable<ProductModel>>> GetProductsAsync() => Result<IEnumerable<ProductModel>>.Success(await _repo.ListAsync());

    public async Task<Result<ProductModel>> GetProductAsync(Guid id)
    {
        var product = await _repo.GetAsync(id);
        return product is null ? Result<ProductModel>.Failure("Not found") : Result<ProductModel>.Success(product);
    }

    public async Task<Result<ProductModel>> CreateProductAsync(ProductModel product)
    {
        if (string.IsNullOrWhiteSpace(product.Title))
            return Result<ProductModel>.Failure("Title required");

        var created = await _repo.CreateAsync(product);
        return Result<ProductModel>.Success(created);
    }

    public async Task<Result<ProductModel>> UpdateProductAsync(Guid id, ProductModel product)
    {
        var updated = await _repo.UpdateAsync(id, product);
        return updated is null ? Result<ProductModel>.Failure("Not found") : Result<ProductModel>.Success(updated);
    }

    public async Task<Result<bool>> DeleteProductAsync(Guid id)
    {
        var deleted = await _repo.DeleteAsync(id);
        return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Not found");
    }
}
