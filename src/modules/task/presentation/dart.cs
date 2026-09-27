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




public static void MapTasks(this IEndpointRouterBuilder endpoints){


    var group=endpoints.MapGroup("/tasks").withTags("Tasks").RequireAuthorization(AuthPolicies.ApiUser);
}
























