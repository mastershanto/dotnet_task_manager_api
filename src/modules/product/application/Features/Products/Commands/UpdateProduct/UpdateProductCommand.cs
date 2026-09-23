using BuildingBlocks.CQRS;
using Products.Domain;

namespace Products.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    string Category
) : ICommand<ProductModel>;
