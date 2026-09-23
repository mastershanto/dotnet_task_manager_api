using Categories.Application.Features.Categories.Commands.CreateCategory;
using Categories.Application.Features.Categories.Commands.DeleteCategory;
using Categories.Application.Features.Categories.Commands.UpdateCategory;
using Categories.Application.Features.Categories.Queries.GetCategories;
using Categories.Application.Features.Categories.Queries.GetCategoryById;
using Categories.Domain;
using BuildingBlocks.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Categories.Presentation;

public record CreateCategoryRequest(
    string Name,
    string? Description,
    string? Color,
    string? Icon
);

public record UpdateCategoryRequest(
    string Name,
    string? Description,
    string? Color,
    string? Icon
);

public static class CategoryEndpoints
{
    public static void MapCategories(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/categories")
            .WithTags("Categories")
            .RequireAuthorization(AuthPolicies.ApiUser);

        // 1. GET ALL (QUERY): /categories
        group.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCategoriesQuery(), cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(string.Join(",", result.Errors));
        })
        .Produces<IEnumerable<CategoryModel>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        // 2. GET BY ID (QUERY): /categories/{id}
        group.MapGet("/{id:guid}", async (ISender sender, Guid id, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCategoryByIdQuery(id), cancellationToken);
            return result.IsSuccess && result.Value is not null
                ? Results.Ok(result.Value)
                : Results.NotFound(new { errors = result.Errors });
        })
        .Produces<CategoryModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // 3. CREATE (COMMAND): /categories
        group.MapPost("/", async (ISender sender, CreateCategoryRequest request, CancellationToken cancellationToken) =>
        {
            var command = new CreateCategoryCommand(
                request.Name,
                request.Description ?? string.Empty,
                request.Color ?? string.Empty,
                request.Icon ?? string.Empty
            );

            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Created($"/categories/{result.Value!.Id}", result.Value)
                : Results.BadRequest(new { errors = result.Errors });
        })
        .Produces<CategoryModel>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status400BadRequest);

        // 4. UPDATE (COMMAND): /categories/{id}
        group.MapPut("/{id:guid}", async (ISender sender, Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken) =>
        {
            var command = new UpdateCategoryCommand(
                id,
                request.Name,
                request.Description ?? string.Empty,
                request.Color ?? string.Empty,
                request.Icon ?? string.Empty
            );

            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    ? Results.NotFound(new { errors = result.Errors })
                    : Results.BadRequest(new { errors = result.Errors }));
        })
        .Produces<CategoryModel>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);

        // 5. DELETE (COMMAND): /categories/{id}
        group.MapDelete("/{id:guid}", async (ISender sender, Guid id, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new DeleteCategoryCommand(id), cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.NotFound(new { errors = result.Errors });
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
