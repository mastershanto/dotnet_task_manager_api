using App.Features.Product.Data;
using App.Features.Product.Domain;

namespace App.Features.Product.Presentation;

public static class ProductEndpoints
{
    public static void MapProducts(this WebApplication app)
    {
        app.MapGet("/products", async (IProductRepository repo) => Results.Ok(await repo.ListAsync()));

        app.MapGet("/products/{id:guid}", async (IProductRepository repo, Guid id) =>
        {
            var product = await repo.GetAsync(id);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        app.MapPost("/products", async (IProductRepository repo, ProductModel product) =>
        {
            var created = await repo.CreateAsync(product);
            return Results.Created($"/products/{created.Id}", created);
        });

        app.MapPut("/products/{id:guid}", async (IProductRepository repo, Guid id, ProductModel product) =>
        {
            var updated = await repo.UpdateAsync(id, product);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        app.MapDelete("/products/{id:guid}", async (IProductRepository repo, Guid id) =>
        {
            return await repo.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
        });
    }
}
