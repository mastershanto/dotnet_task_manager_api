using Users.Application.Features.Users.Commands.CreateUser;
using Users.Application.Features.Users.Commands.DeleteUser;
using Users.Application.Features.Users.Commands.UpdateUser;
using Users.Application.Features.Users.Queries.GetUserById;
using Users.Application.Features.Users.Queries.GetUsers;
using Users.Domain;
using BuildingBlocks.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Presentation;

public record CreateUserRequest(string Name, string Email);
public record UpdateUserRequest(string Name, string Email);

public static class UserEndpoints
{
    public static void MapUsers(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/users")
            .WithTags("Users")
            .RequireAuthorization(AuthPolicies.ApiUser);

        // 1. GET ALL (QUERY): /users
        group.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetUsersQuery(), cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(string.Join(",", result.Errors));
        })
        .Produces<IEnumerable<UserModel>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        // 2. GET BY ID (QUERY): /users/{id}
        group.MapGet("/{id:guid}", async (ISender sender, Guid id, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetUserByIdQuery(id), cancellationToken);
            return result.IsSuccess && result.Value is not null
                ? Results.Ok(result.Value)
                : Results.NotFound(new { errors = result.Errors });
        })
        .Produces<UserModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // 3. CREATE (COMMAND): /users
        group.MapPost("/", async (ISender sender, CreateUserRequest request, CancellationToken cancellationToken) =>
        {
            var command = new CreateUserCommand(request.Name, request.Email);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Created($"/users/{result.Value!.Id}", result.Value)
                : Results.BadRequest(new { errors = result.Errors });
        })
        .Produces<UserModel>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status400BadRequest);

        // 4. UPDATE (COMMAND): /users/{id}
        group.MapPut("/{id:guid}", async (ISender sender, Guid id, UpdateUserRequest request, CancellationToken cancellationToken) =>
        {
            var command = new UpdateUserCommand(id, request.Name, request.Email);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    ? Results.NotFound(new { errors = result.Errors })
                    : Results.BadRequest(new { errors = result.Errors }));
        })
        .Produces<UserModel>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);

        // 5. DELETE (COMMAND): /users/{id}
        group.MapDelete("/{id:guid}", async (ISender sender, Guid id, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new DeleteUserCommand(id), cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.NotFound(new { errors = result.Errors });
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
