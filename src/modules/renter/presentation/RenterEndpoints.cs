using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Renter.Domain;
using Renter.Application.Features.Renter;

namespace Renter.Presentation;

public static class RenterEndpoints
{
    public static void MapRenter(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth/renter")
            .WithTags("Renter");

        group.MapGet("/cities", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetRenterCitiesQuery(), cancellationToken);
            return Results.Ok(new
            {
                success = true,
                message = "Cities retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/repeat-pattern", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetRenterRepeatPatternsQuery(), cancellationToken);
            return Results.Ok(new
            {
                success = true,
                message = "Repeat patterns retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/studios", async (ISender sender, string? city, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetRenterStudiosQuery(city), cancellationToken);
            return Results.Ok(new
            {
                success = true,
                message = "Studios retrieved successfully",
                data = res.Value
            });
        });

        group.MapPost("/check-booking", async (ISender sender, CheckBookingRequest request, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new CheckBookingCommand(request), cancellationToken);
            return Results.Ok(res.Value);
        });

        group.MapPost("/studios/book", async (ISender sender, BookStudioRequest request, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new BookStudioCommand(request), cancellationToken);
            return Results.Ok(new
            {
                success = true,
                message = "Studio booked successfully",
                data = res.Value
            });
        });

        group.MapGet("/my-bookings", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetMyBookingsQuery(), cancellationToken);
            return Results.Ok(new
            {
                success = true,
                message = "My bookings retrieved successfully",
                data = res.Value
            });
        });

        group.MapPost("/bookings/cancel", async (ISender sender, int id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new CancelBookingCommand(id), cancellationToken);
            return Results.Ok(new
            {
                success = true,
                message = "Booking cancelled successfully",
                data = new { cancelled = res.Value }
            });
        });

        group.MapPost("/bookings/cancel-recurring", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new CancelRecurringBookingCommand(), cancellationToken);
            return Results.Ok(new
            {
                success = true,
                message = "Recurring bookings cancelled successfully",
                data = new { cancelled = res.Value }
            });
        });
    }
}
