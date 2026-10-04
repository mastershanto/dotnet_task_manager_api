using BuildingBlocks.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Profile.Application.Features.Profiles.Commands;
using Profile.Application.Features.Profiles.Queries;
using Profile.Application.Features.Profiles.Queries.GetProfile;
using Profile.Domain;

namespace Profile.Presentation;

public static class ProfileEndpoints
{
    public static void MapProfile(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth").WithTags("Profile").AllowAnonymous();

        // GET /auth/profile or /auth/profile?id=9
        group.MapGet("/profile", async (ISender sender, int? id, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetProfileQuery(id), ct);
            return Results.Ok(new { status = true, message = "Profile retrieved successfully", data = result.Value, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // GET /auth/profile/credits
        group.MapGet("/profile/credits", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetCreditsQuery(), ct);
            return Results.Ok(new { status = true, message = "Credits retrieved successfully", data = result.Value, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // GET /auth/profile/low-credit-card
        group.MapGet("/profile/low-credit-card", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetLowCreditCardQuery(), ct);
            return Results.Ok(new { status = true, message = "Low credit card status retrieved", data = result.Value, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // POST /auth/profile/update
        group.MapPost("/profile/update", async (ISender sender, ProfileUpdateDto dto, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateProfileCommand(dto), ct);
            return Results.Ok(new { status = true, message = "Profile updated successfully", data = result.Value, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // POST /auth/profile/toggle-profile-visibility
        group.MapPost("/profile/toggle-profile-visibility", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ToggleProfileVisibilityCommand(), ct);
            return Results.Ok(new { status = true, message = "Profile visibility toggled", data = new { is_profile_visibility = result.Value }, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // POST /auth/profile/update-password
        group.MapPost("/profile/update-password", async (ISender sender, UpdatePasswordDto dto, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdatePasswordCommand(dto), ct);
            return result.IsSuccess
                ? Results.Ok(new { status = true, message = "Password updated successfully", code = 200 })
                : Results.BadRequest(new { status = false, message = string.Join(", ", result.Errors), code = 400 });
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // POST /auth/logout
        group.MapPost("/logout", () =>
        {
            return Results.Ok(new { status = true, message = "Successfully logged out", code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // DELETE /auth/profile/delete
        group.MapDelete("/profile/delete", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteProfileCommand(), ct);
            return Results.Ok(new { status = true, message = "Account deleted successfully", code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // GET /auth/profile/children
        group.MapGet("/profile/children", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetProfileChildrenQuery(), ct);
            return Results.Ok(new { status = true, message = "Children list retrieved", data = result.Value, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // POST /auth/profile/switch-child
        group.MapPost("/profile/switch-child", async (ISender sender, SwitchChildCommand command, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.Value != null
                ? Results.Ok(new { status = true, message = "Switched to child profile", data = result.Value, code = 200 })
                : Results.NotFound(new { status = false, message = "Child profile not found", code = 404 });
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /auth/profile/switch-to-parent
        group.MapPost("/profile/switch-to-parent", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new SwitchToParentCommand(), ct);
            return Results.Ok(new { status = true, message = "Switched to parent profile", data = result.Value, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // GET /auth/cards
        group.MapGet("/cards", async (ISender sender, int? userId, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetCardsQuery(userId), ct);
            return Results.Ok(new { status = true, message = "Cards retrieved successfully", data = result.Value, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // POST /auth/cards
        group.MapPost("/cards", async (ISender sender, StoreCardDto dto, CancellationToken ct) =>
        {
            var result = await sender.Send(new StoreCardCommand(dto), ct);
            return Results.Ok(new { status = true, message = "Card stored successfully", data = result.Value, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // POST /auth/cards/delete?id=12
        group.MapPost("/cards/delete", async (ISender sender, int id, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteCardCommand(id), ct);
            return Results.Ok(new { status = true, message = "Card deleted successfully", code = 200 });
        })
        .Produces(StatusCodes.Status200OK);
    }
}
