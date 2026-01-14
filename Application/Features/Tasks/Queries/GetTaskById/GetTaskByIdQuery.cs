using MediatR;
using TodoApi.Domain.Common;
using TodoApi.Application.Features.Tasks.Commands.CreateTask;

namespace TodoApi.Application.Features.Tasks.Queries.GetTaskById;

/// <summary>
/// Query to get a task by ID - CQRS pattern
/// </summary>
public record GetTaskByIdQuery : IRequest<Result<TaskResponse>>
{
    public int TaskId { get; init; }
    public int UserId { get; init; }
}
