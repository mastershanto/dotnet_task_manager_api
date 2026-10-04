using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using StudentEvents.Domain;
using StudentEvents.Application.Features.StudentEvents;

namespace StudentEvents.Presentation;

public static class StudentEventsEndpoints
{
    public static void MapStudentEvents(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth")
            .WithTags("Student Events & Community");

        group.MapGet("/student/events", async (ISender sender, string? type, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetStudentEventsQuery(type), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Events retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/student/events/details", async (ISender sender, int id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetStudentEventDetailsQuery(id), cancellationToken);
            if (res.Value == null)
                return Results.NotFound(new { status = false, message = "Event not found" });

            return Results.Ok(new
            {
                status = true,
                message = "Event details retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/student/events/participant-list", async (ISender sender, int id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetEventParticipantsQuery(id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Participants retrieved successfully",
                data = res.Value
            });
        });

        group.MapPost("/student/events/book", async (ISender sender, BookEventRequest request, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new BookStudentEventCommand(request), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Event booked successfully",
                data = new { booked = res.Value }
            });
        });

        group.MapGet("/dancer-of-the-month", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetDancerOfTheMonthQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Dancer of the month retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/search-students", async (ISender sender, string? search, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new SearchStudentsQuery(search), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Students search results",
                data = res.Value
            });
        });
    }
}
