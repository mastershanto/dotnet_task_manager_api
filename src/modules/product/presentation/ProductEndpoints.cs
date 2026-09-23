using Products.Application.Features.Products.Commands.CreateProduct;
using Products.Application.Features.Products.Commands.DeleteProduct;
using Products.Application.Features.Products.Commands.UpdateProduct;
using Products.Application.Features.Products.Queries.GetProductById;
using Products.Application.Features.Products.Queries.GetProducts;
using Products.Domain;
using BuildingBlocks.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Products.Presentation;

public record CreateProductRequest(
    string Title,
    string Description,
    decimal Price,
    string Category
);

public record UpdateProductRequest(
    string Title,
    string Description,
    decimal Price,
    string Category
);

public static class ProductEndpoints
{
    public static void MapProducts(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/products")
            .WithTags("Products")
            .RequireAuthorization(AuthPolicies.ApiUser);

        // 1. GET ALL (QUERY): /products
        group.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetProductsQuery(), cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(string.Join(",", result.Errors));
        })
        .Produces<IEnumerable<ProductModel>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        // 2. GET BY ID (QUERY): /products/{id}
        group.MapGet("/{id:guid}", async (ISender sender, Guid id, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetProductByIdQuery(id), cancellationToken);
            return result.IsSuccess && result.Value is not null
                ? Results.Ok(result.Value)
                : Results.NotFound(new { errors = result.Errors });
        })
        .Produces<ProductModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // 3. CREATE (COMMAND): /products
        group.MapPost("/", async (ISender sender, CreateProductRequest request, CancellationToken cancellationToken) =>
        {
            var command = new CreateProductCommand(
                request.Title,
                request.Description,
                request.Price,
                request.Category
            );

            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Created($"/products/{result.Value!.Id}", result.Value)
                : Results.BadRequest(new { errors = result.Errors });
        })
        .Produces<ProductModel>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status400BadRequest);

        // 4. UPDATE (COMMAND): /products/{id}
        group.MapPut("/{id:guid}", async (ISender sender, Guid id, UpdateProductRequest request, CancellationToken cancellationToken) =>
        {
            var command = new UpdateProductCommand(
                id,
                request.Title,
                request.Description,
                request.Price,
                request.Category
            );

            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    ? Results.NotFound(new { errors = result.Errors })
                    : Results.BadRequest(new { errors = result.Errors }));
        })
        .Produces<ProductModel>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);

        // 5. DELETE (COMMAND): /products/{id}
        group.MapDelete("/{id:guid}", async (ISender sender, Guid id, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new DeleteProductCommand(id), cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.NotFound(new { errors = result.Errors });
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
