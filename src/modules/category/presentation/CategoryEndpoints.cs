using Categories.Application;
using Categories.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Categories.Presentation;

public static class CategoryEndpoints
{
    public static void MapCategories(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/categories")
            .WithTags("Categories")
            .RequireAuthorization(AuthPolicies.ApiUser);

        group.MapGet("/", async (ICategoryService categoryService) =>
        {
            var result = await categoryService.GetCategoriesAsync();
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(string.Join(",", result.Errors));
        })
        .Produces<IEnumerable<CategoryModel>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", async (ICategoryService categoryService, Guid id) =>
        {
            var result = await categoryService.GetCategoryAsync(id);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
        })
        .Produces<CategoryModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (ICategoryService categoryService, CategoryModel category) =>
        {
            var validation = Validation.Validate(category).ToArray();
            if (validation.Any())
                return Results.ValidationProblem(Validation.ToErrorDictionary(validation));

            var result = await categoryService.CreateCategoryAsync(category);
            return result.IsSuccess ? Results.Created($"/categories/{result.Value!.Id}", result.Value) : Results.BadRequest(result.Errors);
        })
        .Produces<CategoryModel>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", async (ICategoryService categoryService, Guid id, CategoryModel category) =>
        {
            var validation = Validation.Validate(category).ToArray();
            if (validation.Any())
                return Results.ValidationProblem(Validation.ToErrorDictionary(validation));

            var result = await categoryService.UpdateCategoryAsync(id, category);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
        })
        .Produces<CategoryModel>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (ICategoryService categoryService, Guid id) =>
        {
            var result = await categoryService.DeleteCategoryAsync(id);
            return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Errors);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
