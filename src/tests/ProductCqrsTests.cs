using Products.Application.Features.Products.Commands.CreateProduct;
using Products.Application.Features.Products.Commands.DeleteProduct;
using Products.Application.Features.Products.Commands.UpdateProduct;
using Products.Application.Features.Products.Queries.GetProductById;
using Products.Application.Features.Products.Queries.GetProducts;
using Products.Domain;
using Xunit;

namespace Api.Tests;

public class FakeProductRepository : IProductRepository
{
    private readonly List<ProductModel> _products = new();

    public Task<IEnumerable<ProductModel>> ListAsync() =>
        Task.FromResult<IEnumerable<ProductModel>>(_products.ToList());

    public Task<ProductModel?> GetAsync(Guid id) =>
        Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

    public Task<ProductModel> CreateAsync(ProductModel product)
    {
        _products.Add(product);
        return Task.FromResult(product);
    }

    public Task<ProductModel?> UpdateAsync(Guid id, ProductModel product)
    {
        var idx = _products.FindIndex(p => p.Id == id);
        if (idx == -1) return Task.FromResult<ProductModel?>(null);
        _products[idx] = product;
        return Task.FromResult<ProductModel?>(product);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var item = _products.FirstOrDefault(p => p.Id == id);
        if (item is null) return Task.FromResult(false);
        _products.Remove(item);
        return Task.FromResult(true);
    }
}

public class ProductCqrsTests
{
    [Fact]
    public async Task CreateProductCommand_Success_WhenValid()
    {
        var repo = new FakeProductRepository();
        var handler = new CreateProductCommandHandler(repo);

        var command = new CreateProductCommand("Ballet Shoes", "Professional satin shoes", 49.99m, "Footwear");
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Ballet Shoes", result.Value.Title);
        Assert.Equal(49.99m, result.Value.Price);
    }

    [Fact]
    public void CreateProductValidator_Fails_WhenTitleTooShort()
    {
        var validator = new CreateProductCommandValidator();
        var command = new CreateProductCommand("A", "Desc", 10m, "Cat");

        var validationResult = validator.Validate(command);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(CreateProductCommand.Title));
    }

    [Fact]
    public async Task GetProductsQuery_ReturnsAll()
    {
        var repo = new FakeProductRepository();
        await repo.CreateAsync(new ProductModel { Title = "Prod 1", Description = "Desc", Price = 10, Category = "Cat" });
        await repo.CreateAsync(new ProductModel { Title = "Prod 2", Description = "Desc", Price = 20, Category = "Cat" });

        var handler = new GetProductsQueryHandler(repo);
        var result = await handler.Handle(new GetProductsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task UpdateProductCommand_ReturnsFailure_WhenNotFound()
    {
        var repo = new FakeProductRepository();
        var handler = new UpdateProductCommandHandler(repo);

        var command = new UpdateProductCommand(Guid.NewGuid(), "Updated Title", "Updated Desc", 99.99m, "Updated Cat");
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Errors.First(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteProductCommand_ReturnsSuccess_WhenExists()
    {
        var repo = new FakeProductRepository();
        var existing = await repo.CreateAsync(new ProductModel { Title = "To Delete", Description = "Desc", Price = 10, Category = "Cat" });
        var handler = new DeleteProductCommandHandler(repo);

        var result = await handler.Handle(new DeleteProductCommand(existing.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }
}
