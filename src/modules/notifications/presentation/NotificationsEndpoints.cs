using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Notifications.Domain;
using Notifications.Application.Features.Notifications;

namespace Notifications.Presentation;

public static class NotificationsEndpoints
{
    public static void MapNotifications(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth/notifications")
            .WithTags("Notifications");

        group.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetNotificationsQuery(null), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Notifications retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/major", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetNotificationsQuery("major"), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Major notifications retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/minor", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetNotificationsQuery("minor"), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Minor notifications retrieved successfully",
                data = res.Value
            });
        });

        group.MapPost("/mark-seen", async (ISender sender, int id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new MarkNotificationSeenCommand(id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Notification marked as seen",
                data = new { seen = res.Value }
            });
        });

        group.MapDelete("/delete", async (ISender sender, int id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new DeleteNotificationCommand(id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Notification deleted successfully",
                data = new { deleted = res.Value }
            });
        });
    }
}
