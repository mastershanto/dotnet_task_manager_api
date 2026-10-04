using BuildingBlocks.Abstractions;
using InstructorSyllabus.Application.Features.InstructorSyllabus.Commands;
using InstructorSyllabus.Application.Features.InstructorSyllabus.Queries;
using InstructorSyllabus.Domain;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InstructorSyllabus.Presentation;

public static class InstructorSyllabusEndpoints
{
    public static void MapInstructorSyllabus(this IEndpointRouteBuilder endpoints)
    {
        var instructorGroup = endpoints.MapGroup("/auth/instructor").WithTags("InstructorSyllabus").AllowAnonymous();

        // GET /auth/instructor/syllabi
        instructorGroup.MapGet("/syllabi", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetInstructorSyllabiQuery(), ct);
            return Results.Ok(new
            {
                status = true,
                message = "Syllabi retrieved successfully",
                data = result.Value,
                code = 200
            });
        })
        .Produces(StatusCodes.Status200OK);

        // GET /auth/instructor/student-syllabus?student_id=2&syllabus_id=1
        instructorGroup.MapGet("/student-syllabus", async (ISender sender, int student_id, int syllabus_id, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetStudentSyllabusQuery(student_id, syllabus_id), ct);
            return Results.Ok(new
            {
                status = true,
                message = "Student syllabus retrieved successfully",
                data = result.Value,
                code = 200
            });
        })
        .Produces(StatusCodes.Status200OK);

        // PUT /auth/instructor/student-syllabus/update
        instructorGroup.MapPut("/student-syllabus/update", async (ISender sender, UpdateStudentSyllabusDto dto, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateStudentSyllabusCommand(dto), ct);
            return Results.Ok(new
            {
                status = true,
                message = "Student syllabus progress updated successfully",
                data = result.Value,
                code = 200
            });
        })
        .Produces(StatusCodes.Status200OK);

        // Shared /auth/syllabi routes
        var sharedGroup = endpoints.MapGroup("/auth").WithTags("Syllabus").AllowAnonymous();
        sharedGroup.MapGet("/syllabi", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetInstructorSyllabiQuery(), ct);
            return Results.Ok(new
            {
                status = true,
                message = "Syllabi retrieved successfully",
                data = result.Value,
                code = 200
            });
        })
        .Produces(StatusCodes.Status200OK);

        sharedGroup.MapGet("/syllabi/items", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetStudentSyllabusQuery(2, 1), ct);
            return Results.Ok(new
            {
                status = true,
                message = "Syllabus items retrieved successfully",
                data = result.Value,
                code = 200
            });
        })
        .Produces(StatusCodes.Status200OK);
    }
}
