using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using InstructorCalendar.Application.Features.InstructorCalendar.Queries;

namespace InstructorCalendar.Presentation;

public static class InstructorCalendarEndpoints
{
    public static void MapInstructorCalendar(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth/instructor/calendar")
            .WithTags("Instructor Calendar");

        group.MapGet("/", async (ISender sender, int? instructor_id, int? studio_id, string? date, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCalendarEventsQuery(instructor_id, studio_id, date), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Instructor calendar events retrieved successfully",
                data = result.Value
            });
        });

        group.MapGet("/instructors", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCalendarInstructorsQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Calendar instructors retrieved successfully",
                data = result.Value
            });
        });

        group.MapGet("/studios", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCalendarStudiosQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Calendar studios retrieved successfully",
                data = result.Value
            });
        });
    }
}
