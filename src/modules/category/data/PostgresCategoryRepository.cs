using Categories.Domain;
using Npgsql;

namespace Categories.Data;

public class PostgresCategoryRepository : ICategoryRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresCategoryRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<CategoryModel>> ListAsync()
    {
        const string sql = "SELECT id, name, description, color, icon, created_at FROM categories ORDER BY created_at;";

        var categories = new List<CategoryModel>();
        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            categories.Add(Map(reader));
        }

        return categories;
    }

    public async Task<CategoryModel?> GetAsync(Guid id)
    {
        const string sql = "SELECT id, name, description, color, icon, created_at FROM categories WHERE id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return Map(reader);
    }

    public async Task<CategoryModel> CreateAsync(CategoryModel category)
    {
        var item = category with { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };

        const string sql = """
            INSERT INTO categories (id, name, description, color, icon, created_at)
            VALUES (@id, @name, @description, @color, @icon, @createdAt);
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", item.Id);
        command.Parameters.AddWithValue("name", item.Name);
        command.Parameters.AddWithValue("description", item.Description);
        command.Parameters.AddWithValue("color", item.Color);
        command.Parameters.AddWithValue("icon", item.Icon);
        command.Parameters.AddWithValue("createdAt", item.CreatedAt);

        await command.ExecuteNonQueryAsync();
        return item;
    }

    public async Task<CategoryModel?> UpdateAsync(Guid id, CategoryModel category)
    {
        const string sql = """
            UPDATE categories
            SET name = @name,
                description = @description,
                color = @color,
                icon = @icon
            WHERE id = @id
            RETURNING id, name, description, color, icon, created_at;
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("name", category.Name);
        command.Parameters.AddWithValue("description", category.Description);
        command.Parameters.AddWithValue("color", category.Color);
        command.Parameters.AddWithValue("icon", category.Icon);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return Map(reader);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM categories WHERE id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        var affectedRows = await command.ExecuteNonQueryAsync();
        return affectedRows > 0;
    }

    private static CategoryModel Map(NpgsqlDataReader reader)
    {
        return new CategoryModel
        {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1),
            Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
            Color = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
            Icon = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
            CreatedAt = reader.GetFieldValue<DateTime>(5)
        };
    }
}
