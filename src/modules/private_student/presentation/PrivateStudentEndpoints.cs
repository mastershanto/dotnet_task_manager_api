using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PrivateStudent.Domain;
using PrivateStudent.Application.Features.PrivateStudent;

namespace PrivateStudent.Presentation;

public static class PrivateStudentEndpoints
{
    public static void MapPrivateStudent(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth/privet-student")
            .WithTags("Private Student");

        // Classes
        group.MapGet("/classes", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetPrivateClassesQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Private classes retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/next-private-class", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetNextPrivateClassQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Next private class retrieved successfully",
                data = res.Value
            });
        });

        // Calendar
        group.MapGet("/calendar/instructors", () => Results.Ok(new
        {
            status = true,
            message = "Calendar instructors retrieved successfully",
            data = new[]
            {
                new { id = 1, name = "Carlos Mendez", avatar = "https://i.pravatar.cc/150?u=carlos" },
                new { id = 2, name = "Elena Rostova", avatar = "https://i.pravatar.cc/150?u=elena" }
            }
        }));

        group.MapGet("/calendar/studios", () => Results.Ok(new
        {
            status = true,
            message = "Calendar studios retrieved successfully",
            data = new[]
            {
                new { id = 1, name = "Private Room 1", address = "100 Sunset Blvd, Suite A" },
                new { id = 2, name = "Studio B", address = "100 Sunset Blvd, 2nd Fl" }
            }
        }));

        group.MapGet("/calendar/categories", () => Results.Ok(new
        {
            status = true,
            message = "Calendar categories retrieved successfully",
            data = new[]
            {
                new { id = 1, name = "Tango", color = "#F59E0B" },
                new { id = 2, name = "Salsa", color = "#EF4444" }
            }
        }));

        group.MapGet("/calendar", async (ISender sender, int? category, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetPrivateClassesQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Private calendar retrieved successfully",
                data = res.Value
            });
        });

        // Announcements
        group.MapGet("/announcements", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetAnnouncementsQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Announcements retrieved successfully",
                data = res.Value
            });
        });

        group.MapPost("/announcements/read", async (ISender sender, int id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new ReadAnnouncementCommand(id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Announcement marked as read",
                data = new { read = res.Value }
            });
        });

        // Purchase Programs
        group.MapGet("/purchase-programs", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetPrivatePurchaseProgramsQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Private purchase programs retrieved successfully",
                data = res.Value
            });
        });

        group.MapPost("/purchase-programs/purchase", async (ISender sender, int package_id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new PurchasePrivateProgramCommand(package_id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Package purchased successfully",
                data = new { purchased = res.Value }
            });
        });

        group.MapGet("/purchase-programs/my-packages", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetMyPackagesQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "My packages retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/purchase-programs/myactivepackege", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetMyActivePackageQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Active package retrieved successfully",
                data = res.Value
            });
        });

        // Partner
        group.MapGet("/partner", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetStudentPartnerQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Partner info retrieved successfully",
                data = res.Value
            });
        });

        group.MapPost("/partner/request", async (ISender sender, int user_id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new RequestPartnerCommand(user_id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Partner request sent successfully",
                data = new { requested = res.Value }
            });
        });

        group.MapPost("/partner/cancel", async (ISender sender, int user_id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new CancelPartnerRequestCommand(user_id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Partner request cancelled",
                data = new { cancelled = res.Value }
            });
        });

        group.MapPost("/partner/accept", async (ISender sender, int user_id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new AcceptPartnerCommand(user_id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Partner accepted successfully",
                data = new { accepted = res.Value }
            });
        });

        group.MapPost("/partner/decline", async (ISender sender, int user_id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new DeclinePartnerCommand(user_id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Partner request declined",
                data = new { declined = res.Value }
            });
        });

        group.MapDelete("/partner/remove", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new RemovePartnerCommand(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Partner removed successfully",
                data = new { removed = res.Value }
            });
        });
    }
}
