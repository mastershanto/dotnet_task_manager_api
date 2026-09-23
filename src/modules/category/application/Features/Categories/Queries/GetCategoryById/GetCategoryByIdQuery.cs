using BuildingBlocks.CQRS;
using Categories.Domain;

namespace Categories.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IQuery<CategoryModel?>;
