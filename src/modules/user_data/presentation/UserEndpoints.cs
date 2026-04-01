using App.Features.User.Application;
using App.Features.User.Domain;
using Shared;

namespace App.Features.User.Presentation;

public static class UserEndpoints
{
    public static void MapUsers(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/users").WithTags("Users");

        group.MapGet("/", async (IUserService userService) =>
        {
            var result = await userService.GetUsersAsync();
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(string.Join(",", result.Errors));
        })
        .Produces<IEnumerable<UserModel>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", async (IUserService userService, Guid id) =>
        {
            var result = await userService.GetUserAsync(id);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
        })
        .Produces<UserModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (IUserService userService, UserModel user) =>
        {
            var validation = Validation.Validate(user).ToArray();
            if (validation.Any())
                return Results.ValidationProblem(Validation.ToErrorDictionary(validation));

            var result = await userService.CreateUserAsync(user);
            return result.IsSuccess ? Results.Created($"/users/{result.Value!.Id}", result.Value) : Results.BadRequest(result.Errors);
        })
        .Produces<UserModel>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", async (IUserService userService, Guid id, UserModel user) =>
        {
            var validation = Validation.Validate(user).ToArray();
            if (validation.Any())
                return Results.ValidationProblem(Validation.ToErrorDictionary(validation));

            var result = await userService.UpdateUserAsync(id, user);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
        })
        .Produces<UserModel>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (IUserService userService, Guid id) =>
        {
            var result = await userService.DeleteUserAsync(id);
            return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Errors);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
