using BuildingBlocks.CQRS;
using Categories.Domain;

namespace Categories.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery : IQuery<IEnumerable<CategoryModel>>;
