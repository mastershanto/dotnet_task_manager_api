using App.Features.User.Application;
using App.Features.User.Domain;

namespace App.Features.User.Presentation;

public static class UserEndpoints
{
    public static void MapUsers(this WebApplication app)
    {
        app.MapGet("/users", async (IUserService userService) =>
        {
            var result = await userService.GetUsersAsync();
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(string.Join(",", result.Errors));
        });

        app.MapGet("/users/{id:guid}", async (IUserService userService, Guid id) =>
        {
            var result = await userService.GetUserAsync(id);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
        });

        app.MapPost("/users", async (IUserService userService, UserModel user) =>
        {
            var result = await userService.CreateUserAsync(user);
            return result.IsSuccess ? Results.Created($"/users/{result.Value!.Id}", result.Value) : Results.BadRequest(result.Errors);
        });

        app.MapPut("/users/{id:guid}", async (IUserService userService, Guid id, UserModel user) =>
        {
            var result = await userService.UpdateUserAsync(id, user);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
        });

        app.MapDelete("/users/{id:guid}", async (IUserService userService, Guid id) =>
        {
            var result = await userService.DeleteUserAsync(id);
            return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Errors);
        });
    }
}
