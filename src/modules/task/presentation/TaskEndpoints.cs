using BuildingBlocks.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Tasks.Application.Features.Tasks.Commands.CreateTask;
using Tasks.Application.Features.Tasks.Commands.DeleteTask;
using Tasks.Application.Features.Tasks.Commands.UpdateTask;
using Tasks.Application.Features.Tasks.Queries.GetTaskById;
using Tasks.Application.Features.Tasks.Queries.GetTasks;
using Tasks.Domain;

namespace Tasks.Presentation;

/// <summary>
/// DTO for updating task via HTTP PUT:
/// </summary>
public record UpdateTaskRequest(
    string Title,
    string Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid? CategoryId,
    Guid? AssignedUserId
);

/// <summary>
/// Task Minimal API Endpoints (CQRS Presentation Layer):
/// এন্ডপয়েন্টগুলো সরাসরি কোনো বিজনেস লজিক রাখে না, বরং MediatR (ISender)-এর মাধ্যমে
/// কমান্ড ও কুয়েরি ডিসপ্যাচ করে (World-Class Clean Architecture)।
/// </summary>
public static class TaskEndpoints
{
    public static void MapTasks(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/tasks")
            .WithTags("Tasks")
            .RequireAuthorization(AuthPolicies.ApiUser);

        // 1. GET ALL (QUERY): /tasks
        group.MapGet("/", async (
            ISender sender,
            TaskItemStatus? status,
            Guid? categoryId,
            CancellationToken cancellationToken) =>
        {
            var query = new GetTasksQuery(status, categoryId);
            var result = await sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(string.Join(", ", result.Errors));
        })
        .Produces<IEnumerable<TaskItemModel>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        // 2. GET BY ID (QUERY): /tasks/{id}
        group.MapGet("/{id:guid}", async (ISender sender, Guid id, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetTaskByIdQuery(id), cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result.Errors);
        })
        .Produces<TaskItemModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // 3. CREATE (COMMAND): /tasks
        group.MapPost("/", async (ISender sender, CreateTaskCommand command, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Created($"/tasks/{result.Value!.Id}", result.Value)
                : Results.BadRequest(result.Errors);
        })
        .Produces<TaskItemModel>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // 4. UPDATE (COMMAND): /tasks/{id}
        group.MapPut("/{id:guid}", async (
            ISender sender,
            Guid id,
            UpdateTaskRequest request,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateTaskCommand(
                id,
                request.Title,
                request.Description,
                request.Status,
                request.Priority,
                request.DueDate,
                request.CategoryId,
                request.AssignedUserId
            );

            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result.Errors);
        })
        .Produces<TaskItemModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        // 5. DELETE (COMMAND): /tasks/{id}
        group.MapDelete("/{id:guid}", async (ISender sender, Guid id, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new DeleteTaskCommand(id), cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.NotFound(result.Errors);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
