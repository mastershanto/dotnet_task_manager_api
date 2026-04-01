using App.Features.User.Domain;
using Npgsql;

namespace App.Features.User.Data;

public class PostgresUserRepository : IUserRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresUserRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<UserModel>> ListAsync()
    {
        const string sql = "SELECT id, name, email, created_at FROM users ORDER BY created_at;";

        var users = new List<UserModel>();
        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(Map(reader));
        }

        return users;
    }

    public async Task<UserModel?> GetAsync(Guid id)
    {
        const string sql = "SELECT id, name, email, created_at FROM users WHERE id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return Map(reader);
    }

    public async Task<UserModel> CreateAsync(UserModel user)
    {
        var item = user with { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };

        const string sql = """
            INSERT INTO users (id, name, email, created_at)
            VALUES (@id, @name, @email, @createdAt);
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", item.Id);
        command.Parameters.AddWithValue("name", item.Name);
        command.Parameters.AddWithValue("email", item.Email);
        command.Parameters.AddWithValue("createdAt", item.CreatedAt);

        await command.ExecuteNonQueryAsync();
        return item;
    }

    public async Task<UserModel?> UpdateAsync(Guid id, UserModel user)
    {
        const string sql = """
            UPDATE users
            SET name = @name,
                email = @email
            WHERE id = @id
            RETURNING id, name, email, created_at;
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("name", user.Name);
        command.Parameters.AddWithValue("email", user.Email);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return Map(reader);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM users WHERE id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        var affectedRows = await command.ExecuteNonQueryAsync();
        return affectedRows > 0;
    }

    private static UserModel Map(NpgsqlDataReader reader)
    {
        return new UserModel
        {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1),
            Email = reader.GetString(2),
            CreatedAt = reader.GetFieldValue<DateTime>(3)
        };
    }
}
