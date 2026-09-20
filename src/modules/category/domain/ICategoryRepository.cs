namespace Categories.Domain;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryModel>> ListAsync();
    Task<CategoryModel?> GetAsync(Guid id);
    Task<CategoryModel> CreateAsync(CategoryModel category);
    Task<CategoryModel?> UpdateAsync(Guid id, CategoryModel category);
    Task<bool> DeleteAsync(Guid id);
}
