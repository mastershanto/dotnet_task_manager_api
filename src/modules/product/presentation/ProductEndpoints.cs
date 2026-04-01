using Products.Application;
using Products.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.Security;
using Microsoft.AspNetCore.Builder;

namespace Products.Presentation;

public static class ProductEndpoints
{
    public static void MapProducts(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/products")
            .WithTags("Products")
            .RequireAuthorization(AuthPolicies.ApiUser);

        group.MapGet("/", async (IProductService productService) =>
        {
            var result = await productService.GetProductsAsync();
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(string.Join(",", result.Errors));
        })
        .Produces<IEnumerable<ProductModel>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", async (IProductService productService, Guid id) =>
        {
            var result = await productService.GetProductAsync(id);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
        })
        .Produces<ProductModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (IProductService productService, ProductModel product) =>
        {
            var validation = Validation.Validate(product).ToArray();
            if (validation.Any())
                return Results.ValidationProblem(Validation.ToErrorDictionary(validation));

            var result = await productService.CreateProductAsync(product);
            return result.IsSuccess ? Results.Created($"/products/{result.Value!.Id}", result.Value) : Results.BadRequest(result.Errors);
        })
        .Produces<ProductModel>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", async (IProductService productService, Guid id, ProductModel product) =>
        {
            var validation = Validation.Validate(product).ToArray();
            if (validation.Any())
                return Results.ValidationProblem(Validation.ToErrorDictionary(validation));

            var result = await productService.UpdateProductAsync(id, product);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
        })
        .Produces<ProductModel>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (IProductService productService, Guid id) =>
        {
            var result = await productService.DeleteProductAsync(id);
            return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Errors);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
