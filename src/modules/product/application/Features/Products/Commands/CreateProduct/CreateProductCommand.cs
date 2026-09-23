using BuildingBlocks.CQRS;
using Products.Domain;

namespace Products.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Title,
    string Description,
    decimal Price,
    string Category
) : ICommand<ProductModel>;
