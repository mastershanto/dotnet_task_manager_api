using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using StudentChildren.Domain;
using StudentChildren.Application.Features.StudentChildren;

namespace StudentChildren.Presentation;

public static class StudentChildrenEndpoints
{
    public static void MapStudentChildren(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth/student/children")
            .WithTags("Student Children");

        group.MapGet("/", async (ISender sender, int? child_id, CancellationToken cancellationToken) =>
        {
            if (child_id.HasValue && child_id.Value > 0)
            {
                var single = await sender.Send(new GetChildByIdQuery(child_id.Value), cancellationToken);
                if (single.Value == null)
                    return Results.NotFound(new { status = false, message = "Child not found" });

                return Results.Ok(new
                {
                    status = true,
                    message = "Child profile retrieved successfully",
                    data = single.Value
                });
            }

            var list = await sender.Send(new GetChildrenQuery(), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Children retrieved successfully",
                data = list.Value
            });
        });

        group.MapPost("/create", async (ISender sender, CreateStudentKidDto dto, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new CreateChildCommand(dto), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Child registered successfully",
                data = res.Value
            });
        });

        group.MapPut("/", async (ISender sender, int child_id, UpdateStudentKidDto dto, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new UpdateChildCommand(child_id, dto), cancellationToken);
            if (res.Value == null)
                return Results.NotFound(new { status = false, message = "Child not found" });

            return Results.Ok(new
            {
                status = true,
                message = "Child updated successfully",
                data = res.Value
            });
        });

        group.MapDelete("/", async (ISender sender, int child_id, CancellationToken cancellationToken) =>
        {
            var res = await sender.Send(new DeleteChildCommand(child_id), cancellationToken);
            return Results.Ok(new
            {
                status = true,
                message = "Child deleted successfully",
                data = new { deleted = res.Value }
            });
        });
    }
}
