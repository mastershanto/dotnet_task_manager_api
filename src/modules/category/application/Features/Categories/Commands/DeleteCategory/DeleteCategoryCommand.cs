using BuildingBlocks.CQRS;

namespace Categories.Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : ICommand;
