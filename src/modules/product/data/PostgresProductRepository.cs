using App.Features.Product.Domain;
using Npgsql;

namespace App.Features.Product.Data;

public class PostgresProductRepository : IProductRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresProductRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<ProductModel>> ListAsync()
    {
        const string sql = "SELECT id, title, description, price, category, created_at FROM products ORDER BY created_at;";

        var products = new List<ProductModel>();
        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(Map(reader));
        }

        return products;
    }

    public async Task<ProductModel?> GetAsync(Guid id)
    {
        const string sql = "SELECT id, title, description, price, category, created_at FROM products WHERE id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return Map(reader);
    }

    public async Task<ProductModel> CreateAsync(ProductModel product)
    {
        var item = product with { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };

        const string sql = """
            INSERT INTO products (id, title, description, price, category, created_at)
            VALUES (@id, @title, @description, @price, @category, @createdAt);
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", item.Id);
        command.Parameters.AddWithValue("title", item.Title);
        command.Parameters.AddWithValue("description", item.Description);
        command.Parameters.AddWithValue("price", item.Price);
        command.Parameters.AddWithValue("category", item.Category);
        command.Parameters.AddWithValue("createdAt", item.CreatedAt);

        await command.ExecuteNonQueryAsync();
        return item;
    }

    public async Task<ProductModel?> UpdateAsync(Guid id, ProductModel product)
    {
        const string sql = """
            UPDATE products
            SET title = @title,
                description = @description,
                price = @price,
                category = @category
            WHERE id = @id
            RETURNING id, title, description, price, category, created_at;
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("title", product.Title);
        command.Parameters.AddWithValue("description", product.Description);
        command.Parameters.AddWithValue("price", product.Price);
        command.Parameters.AddWithValue("category", product.Category);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return Map(reader);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM products WHERE id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        var affectedRows = await command.ExecuteNonQueryAsync();
        return affectedRows > 0;
    }

    private static ProductModel Map(NpgsqlDataReader reader)
    {
        return new ProductModel
        {
            Id = reader.GetGuid(0),
            Title = reader.GetString(1),
            Description = reader.GetString(2),
            Price = reader.GetDecimal(3),
            Category = reader.GetString(4),
            CreatedAt = reader.GetFieldValue<DateTime>(5)
        };
    }
}
