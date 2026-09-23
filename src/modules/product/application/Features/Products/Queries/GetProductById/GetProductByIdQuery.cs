using BuildingBlocks.CQRS;
using Products.Domain;

namespace Products.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<ProductModel?>;
