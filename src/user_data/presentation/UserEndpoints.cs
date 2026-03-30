using App.Features.User.Data;
using App.Features.User.Domain;

namespace App.Features.User.Presentation;

public static class UserEndpoints
{
    public static void MapUsers(this WebApplication app)
    {
        app.MapGet("/users", async (IUserRepository userRepository) => Results.Ok(await userRepository.ListAsync()));

        app.MapGet("/users/{id:guid}", async (IUserRepository userRepository, Guid id) =>
        {
            var found = await userRepository.GetAsync(id);
            return found is null ? Results.NotFound() : Results.Ok(found);
        });

        app.MapPost("/users", async (IUserRepository userRepository, UserModel user) =>
        {
            var created = await userRepository.CreateAsync(user);
            return Results.Created($"/users/{created.Id}", created);
        });

        app.MapPut("/users/{id:guid}", async (IUserRepository userRepository, Guid id, UserModel user) =>
        {
            var updated = await userRepository.UpdateAsync(id, user);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        app.MapDelete("/users/{id:guid}", async (IUserRepository userRepository, Guid id) =>
        {
            return await userRepository.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
        });
    }
}
