using BuildingBlocks.Abstractions;
using MediatR;

namespace BuildingBlocks.CQRS;

/// <summary>
/// CQRS Query Handler interface for queries returning Result<TResponse>.
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
