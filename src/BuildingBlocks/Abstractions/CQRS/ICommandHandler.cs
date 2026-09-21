using BuildingBlocks.Abstractions;
using MediatR;

namespace BuildingBlocks.CQRS;

/// <summary>
/// CQRS Command Handler interface for commands returning Result<TResponse>.
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}

/// <summary>
/// CQRS Command Handler interface for commands returning Result<bool>.
/// </summary>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result<bool>>
    where TCommand : ICommand
{
}
