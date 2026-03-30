using App.Features.Product.Application;
using App.Features.Product.Domain;

namespace App.Features.Product.Presentation;

public static class ProductEndpoints
{
    public static void MapProducts(this WebApplication app)
    {
        app.MapGet("/products", async (IProductService productService) =>
        {
            var result = await productService.GetProductsAsync();
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(string.Join(",", result.Errors));
        });

        app.MapGet("/products/{id:guid}", async (IProductService productService, Guid id) =>
        {
            var result = await productService.GetProductAsync(id);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
        });

        app.MapPost("/products", async (IProductService productService, ProductModel product) =>
        {
            var result = await productService.CreateProductAsync(product);
            return result.IsSuccess ? Results.Created($"/products/{result.Value!.Id}", result.Value) : Results.BadRequest(result.Errors);
        });

        app.MapPut("/products/{id:guid}", async (IProductService productService, Guid id, ProductModel product) =>
        {
            var result = await productService.UpdateProductAsync(id, product);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
        });

        app.MapDelete("/products/{id:guid}", async (IProductService productService, Guid id) =>
        {
            var result = await productService.DeleteProductAsync(id);
            return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Errors);
        });
    }
}
