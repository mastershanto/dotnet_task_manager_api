using BuildingBlocks.CQRS;
using Categories.Domain;

namespace Categories.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string Description = "",
    string Color = "",
    string Icon = ""
) : ICommand<CategoryModel>;
