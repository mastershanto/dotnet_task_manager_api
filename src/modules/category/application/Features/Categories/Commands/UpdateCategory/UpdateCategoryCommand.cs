using BuildingBlocks.CQRS;
using Categories.Domain;

namespace Categories.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Description = "",
    string Color = "",
    string Icon = ""
) : ICommand<CategoryModel>;
