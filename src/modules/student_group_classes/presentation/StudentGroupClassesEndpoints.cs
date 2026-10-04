using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using StudentGroupClasses.Application.Features.StudentGroupClasses.Queries;
using StudentGroupClasses.Application.Features.StudentGroupClasses.Commands;

namespace StudentGroupClasses.Presentation;

public record EnrollClassRequest(int ClassId);

public static class StudentGroupClassesEndpoints
{
    public static void MapStudentGroupClasses(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth/student")
            .WithTags("Student Group Classes");

        // Classes
        group.MapGet("/group-classes", async (ISender sender, string? date, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetStudentGroupClassesQuery(date), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Group classes retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/my-classes", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetStudentMyClassesQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "My classes retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/group-classes/class-history", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetStudentClassHistoryQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Class history retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/group-classes/details", async (ISender sender, int class_id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetStudentClassDetailsQuery(class_id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Class details retrieved successfully",
                data = res.Value
            });
        });

        group.MapPost("/group-classes/enroll", async (ISender sender, EnrollClassRequest? body, int? class_id, CancellationToken cancellationToken) =>
        {
            var targetId = class_id ?? body?.ClassId ?? 0;
            var res = await sender.Send(new EnrollGroupClassCommand(targetId), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Enrolled in class successfully",
                data = new { enrolled = res.Value }
            });
        });

        group.MapPost("/group-classes/cancel", async (ISender sender, int class_id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new CancelGroupClassCommand(class_id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Class enrollment cancelled successfully",
                data = new { cancelled = res.Value }
            });
        });

        group.MapPost("/group-classes/cancel-recurring", async (ISender sender, int class_id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new CancelRecurringGroupClassCommand(class_id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Recurring enrollment cancelled successfully",
                data = new { cancelled = res.Value }
            });
        });

        // Class Guides
        group.MapGet("/group-classes/class-guide", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetStudentClassGuidesQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Class guides retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/group-classes/class-guide-details", async (ISender sender, int id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetStudentClassGuideDetailsQuery(id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Class guide details retrieved successfully",
                data = res.Value
            });
        });

        // Calendar sub-routes
        group.MapGet("/group-classes/calendar/instructors", () => Results.Ok(new
        {
            status = true,
            message = "Calendar instructors retrieved successfully",
            data = new[]
            {
                new { id = 1, name = "Carlos Mendez", avatar = "https://i.pravatar.cc/150?u=carlos" },
                new { id = 2, name = "Elena Rostova", avatar = "https://i.pravatar.cc/150?u=elena" }
            }
        }));

        group.MapGet("/group-classes/calendar/studios", () => Results.Ok(new
        {
            status = true,
            message = "Calendar studios retrieved successfully",
            data = new[]
            {
                new { id = 1, name = "Main Dance Hall", address = "100 Sunset Blvd" },
                new { id = 2, name = "Studio B", address = "100 Sunset Blvd, 2nd Fl" }
            }
        }));

        group.MapGet("/group-classes/calendar/categories", () => Results.Ok(new
        {
            status = true,
            message = "Calendar categories retrieved successfully",
            data = new[]
            {
                new { id = 1, name = "Salsa", color = "#EF4444" },
                new { id = 2, name = "Bachata", color = "#8B5CF6" },
                new { id = 3, name = "Tango", color = "#F59E0B" }
            }
        }));

        group.MapGet("/group-classes/calendar", async (ISender sender, string? date, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetStudentGroupClassesQuery(date), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Group class calendar retrieved successfully",
                data = res.Value
            });
        });

        // Purchase Programs
        group.MapGet("/purchase-programs", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetStudentPurchaseProgramsQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Purchase programs retrieved successfully",
                data = res.Value
            });
        });

        group.MapPost("/purchase-programs/purchase", async (ISender sender, int id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new PurchaseProgramCommand(id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Program purchased successfully",
                data = new { purchased = res.Value }
            });
        });

        // My Calendar & Google Sync
        group.MapGet("/my-calendar", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new GetStudentMyCalendarQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "My calendar retrieved successfully",
                data = res.Value
            });
        });

        group.MapGet("/sync-google-calendar", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new SyncGoogleCalendarQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Google calendar synced successfully",
                data = res.Value
            });
        });
    }
}
