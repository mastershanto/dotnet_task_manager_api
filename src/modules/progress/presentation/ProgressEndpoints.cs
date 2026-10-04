using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Progress.Domain;
using Progress.Application.Features.Progress;

namespace Progress.Presentation;

public static class ProgressEndpoints
{
    public static void MapProgress(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth/progress")
            .WithTags("Student Progress");

        group.MapGet("/", async (ISender sender, int? id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetStudentProgressQuery(id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Progress retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/recent-achievements", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetRecentAchievementsQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Recent achievements retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/recent-milestones", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetRecentMilestonesQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Recent milestones retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/streak", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetStreakQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Streak info retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/level", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetLevelQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Level progress retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/about-me", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetAboutMeQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "About me retrieved successfully",
                data = new { about_me = res.Value }
            });
        });

        group.MapPost("/about-me/update", async (ISender sender, UpdateAboutMeRequest request, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new UpdateAboutMeCommand(request.AboutMe), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "About me updated successfully",
                data = new { updated = res.Value }
            });
        });
    }
}
