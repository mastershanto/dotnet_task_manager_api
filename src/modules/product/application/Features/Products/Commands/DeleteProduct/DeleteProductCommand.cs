using BuildingBlocks.CQRS;

namespace Products.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : ICommand;
