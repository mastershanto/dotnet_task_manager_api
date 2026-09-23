using BuildingBlocks.CQRS;
using Products.Domain;

namespace Products.Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery : IQuery<IEnumerable<ProductModel>>;
