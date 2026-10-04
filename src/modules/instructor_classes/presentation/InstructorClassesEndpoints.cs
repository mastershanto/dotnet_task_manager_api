using BuildingBlocks.Abstractions;
using InstructorClasses.Application.Features.InstructorClasses.Commands;
using InstructorClasses.Application.Features.InstructorClasses.Queries;
using InstructorClasses.Domain;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InstructorClasses.Presentation;

public static class InstructorClassesEndpoints
{
    public static void MapInstructorClasses(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth/instructor").WithTags("InstructorClasses").AllowAnonymous();

        // GET /auth/instructor/classes?date=2026-09-12
        group.MapGet("/classes", async (ISender sender, string? date, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetInstructorClassesQuery(date), ct);
            return Results.Ok(new
            {
                status = true,
                message = "Instructor classes retrieved successfully",
                data = result.Value,
                code = 200
            });
        })
        .Produces(StatusCodes.Status200OK);

        // GET /auth/instructor/class?lesson_id=1
        group.MapGet("/class", async (ISender sender, int lesson_id, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetInstructorClassByIdQuery(lesson_id), ct);
            return result.Value != null
                ? Results.Ok(new { status = true, message = "Class retrieved successfully", data = result.Value, code = 200 })
                : Results.NotFound(new { status = false, message = "Class not found", code = 404 });
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // PUT /auth/instructor/classes/update/attendance?lesson_id=1&student_id=16
        group.MapPut("/classes/update/attendance", async (ISender sender, int lesson_id, int student_id, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateAttendanceCommand(lesson_id, student_id), ct);
            return result.IsSuccess
                ? Results.Ok(new { status = true, message = "Attendance updated successfully", data = result.Value, code = 200 })
                : Results.BadRequest(new { status = false, message = string.Join(", ", result.Errors), code = 400 });
        })
        .Produces(StatusCodes.Status200OK);

        // GET /auth/instructor/classes/details/student?lesson_id=1&student_id=16
        group.MapGet("/classes/details/student", async (ISender sender, int lesson_id, int student_id, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetClassStudentDetailsQuery(lesson_id, student_id), ct);
            return Results.Ok(new { status = true, message = "Student class details retrieved", data = result.Value, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // GET /auth/instructor/repeat-patterns
        group.MapGet("/repeat-patterns", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetRepeatPatternsQuery(), ct);
            return Results.Ok(new { status = true, message = "Repeat patterns retrieved", data = result.Value, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // POST /auth/instructor/classes/create
        group.MapPost("/classes/create", async (ISender sender, CreateClassDto dto, CancellationToken ct) =>
        {
            var result = await sender.Send(new CreateClassCommand(dto), ct);
            return Results.Ok(new { status = true, message = "Class created successfully", data = result.Value, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // DELETE /auth/instructor/classes/delete?class_id=2
        group.MapDelete("/classes/delete", async (ISender sender, int class_id, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteClassCommand(class_id), ct);
            return Results.Ok(new { status = true, message = "Class deleted successfully", code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // POST /auth/instructor/classes/add-instructor
        group.MapPost("/classes/add-instructor", async (ISender sender, AddInstructorDto dto, CancellationToken ct) =>
        {
            var result = await sender.Send(new AddInstructorCommand(dto), ct);
            return Results.Ok(new { status = true, message = "Instructor added to class", code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // POST /auth/instructor/classes/add-student
        group.MapPost("/classes/add-student", async (ISender sender, AddStudentDto dto, CancellationToken ct) =>
        {
            var result = await sender.Send(new AddStudentCommand(dto), ct);
            return Results.Ok(new { status = true, message = "Student added to class", code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // GET /auth/instructor/students?q=Amara
        group.MapGet("/students", async (ISender sender, string? q, CancellationToken ct) =>
        {
            var result = await sender.Send(new SearchStudentsQuery(q), ct);
            return Results.Ok(new { status = true, message = "Students retrieved", data = result.Value, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // GET /auth/instructor/instructors?q=Carlos
        group.MapGet("/instructors", async (ISender sender, string? q, CancellationToken ct) =>
        {
            var result = await sender.Send(new SearchInstructorsQuery(q), ct);
            return Results.Ok(new { status = true, message = "Instructors retrieved", data = result.Value, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // GET /auth/instructor/studios?q=River
        group.MapGet("/studios", async (ISender sender, string? q, CancellationToken ct) =>
        {
            var result = await sender.Send(new SearchStudiosQuery(q), ct);
            return Results.Ok(new { status = true, message = "Studios retrieved", data = result.Value, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);
    }
}
