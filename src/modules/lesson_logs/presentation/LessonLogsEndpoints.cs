using BuildingBlocks.Abstractions;
using LessonLogs.Application.Features.LessonLogs.Commands;
using LessonLogs.Application.Features.LessonLogs.Queries.GetLessonLogs;
using LessonLogs.Domain;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LessonLogs.Presentation;

public static class LessonLogsEndpoints
{
    public static void MapLessonLogs(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth/lesson-logs").WithTags("LessonLogs").AllowAnonymous();

        // GET /auth/lesson-logs?user_id={id}&date={date}
        group.MapGet("/", async (ISender sender, int? user_id, string? date, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetLessonLogsQuery(user_id, date), ct);
            return Results.Ok(new
            {
                status = true,
                message = "Lesson logs retrieved successfully",
                data = result.Value,
                code = 200
            });
        })
        .Produces(StatusCodes.Status200OK);

        // POST /auth/lesson-logs
        group.MapPost("/", async (ISender sender, CreateLessonLogDto dto, CancellationToken ct) =>
        {
            var result = await sender.Send(new CreateLessonLogCommand(dto), ct);
            return Results.Ok(new
            {
                status = true,
                message = "Lesson log created successfully",
                data = result.Value,
                code = 200
            });
        })
        .Produces(StatusCodes.Status200OK);

        // PUT /auth/lesson-logs/{id}
        group.MapPut("/{id:int}", async (ISender sender, int id, UpdateLessonLogDto dto, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateLessonLogCommand(id, dto), ct);
            return result.IsSuccess
                ? Results.Ok(new { status = true, message = "Lesson log updated successfully", data = result.Value, code = 200 })
                : Results.NotFound(new { status = false, message = "Lesson log not found", code = 404 });
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // DELETE /auth/lesson-logs/{id}
        group.MapDelete("/{id:int}", async (ISender sender, int id, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteLessonLogCommand(id), ct);
            return Results.Ok(new { status = true, message = "Lesson log deleted successfully", code = 200 });
        })
        .Produces(StatusCodes.Status200OK);
    }
}
