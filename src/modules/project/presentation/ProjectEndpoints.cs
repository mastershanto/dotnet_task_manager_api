using BuildingBlocks.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Projects.Application.Features.Projects.Commands.CreateProject;
using Projects.Application.Features.Projects.Commands.DeleteProject;
using Projects.Application.Features.Projects.Commands.UpdateProject;
using Projects.Application.Features.Projects.Queries.GetProjectById;
using Projects.Application.Features.Projects.Queries.GetProjects;
using Projects.Domain;

namespace Projects.Presentation;

/// <summary>
/// DTO for updating project via HTTP PUT:
/// </summary>
public record UpdateProjectRequest(
    string Name,
    string Description,
    ProjectStatus Status,
    string Color,
    DateTime? StartDate,
    DateTime? EndDate,
    Guid? OwnerId
);

/// <summary>
/// Project Minimal API Endpoints (CQRS Presentation Layer):
/// </summary>
public static class ProjectEndpoints
{
    public static void MapProjects(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/projects")
            .WithTags("Projects")
            .RequireAuthorization(AuthPolicies.ApiUser);

        // 1. GET ALL (QUERY): /projects
        group.MapGet("/", async (
            ISender sender,
            ProjectStatus? status,
            CancellationToken cancellationToken) =>
        {
            var query = new GetProjectsQuery(status);
            var result = await sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(string.Join(", ", result.Errors));
        })
        .Produces<IEnumerable<ProjectModel>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        // 2. GET BY ID (QUERY): /projects/{id}
        group.MapGet("/{id:guid}", async (ISender sender, Guid id, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetProjectByIdQuery(id), cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result.Errors);
        })
        .Produces<ProjectModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // 3. CREATE (COMMAND): /projects
        group.MapPost("/", async (ISender sender, CreateProjectCommand command, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Created($"/projects/{result.Value!.Id}", result.Value)
                : Results.BadRequest(result.Errors);
        })
        .Produces<ProjectModel>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // 4. UPDATE (COMMAND): /projects/{id}
        group.MapPut("/{id:guid}", async (
            ISender sender,
            Guid id,
            UpdateProjectRequest request,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateProjectCommand(
                id,
                request.Name,
                request.Description,
                request.Status,
                request.Color,
                request.StartDate,
                request.EndDate,
                request.OwnerId
            );

            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result.Errors);
        })
        .Produces<ProjectModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        // 5. DELETE (COMMAND): /projects/{id}
        group.MapDelete("/{id:guid}", async (ISender sender, Guid id, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new DeleteProjectCommand(id), cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.NotFound(result.Errors);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
