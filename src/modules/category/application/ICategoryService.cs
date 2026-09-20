using Categories.Domain;
using BuildingBlocks.Abstractions;

namespace Categories.Application;

public interface ICategoryService
{
    Task<Result<IEnumerable<CategoryModel>>> GetCategoriesAsync();
    Task<Result<CategoryModel>> GetCategoryAsync(Guid id);
    Task<Result<CategoryModel>> CreateCategoryAsync(CategoryModel category);
    Task<Result<CategoryModel>> UpdateCategoryAsync(Guid id, CategoryModel category);
    Task<Result<bool>> DeleteCategoryAsync(Guid id);
}
