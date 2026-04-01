using Microsoft.Extensions.Options;
using Npgsql;

namespace Api.Infrastructure.Persistence;

public sealed class PostgresMigrationRunner
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly PersistenceOptions _options;
    private readonly ILogger<PostgresMigrationRunner> _logger;

    public PostgresMigrationRunner(
        NpgsqlDataSource dataSource,
        IHostEnvironment hostEnvironment,
        IOptions<PersistenceOptions> options,
        ILogger<PostgresMigrationRunner> logger)
    {
        _dataSource = dataSource;
        _hostEnvironment = hostEnvironment;
        _options = options.Value;
        _logger = logger;
    }

    public async Task ApplyMigrationsAsync(CancellationToken cancellationToken = default)
    {
        var migrationsPath = ResolveMigrationsPath();
        if (!Directory.Exists(migrationsPath))
        {
            _logger.LogWarning("Migration path not found: {MigrationsPath}", migrationsPath);
            return;
        }

        var migrationFiles = Directory.GetFiles(migrationsPath, "*.sql", SearchOption.TopDirectoryOnly)
            .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (migrationFiles.Length == 0)
        {
            _logger.LogInformation("No SQL migration files found at {MigrationsPath}", migrationsPath);
            return;
        }

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await EnsureMigrationsTableAsync(connection, cancellationToken);

        var applied = await LoadAppliedMigrationsAsync(connection, cancellationToken);

        foreach (var file in migrationFiles)
        {
            var migrationName = Path.GetFileName(file);
            if (applied.Contains(migrationName))
            {
                continue;
            }

            var sql = await File.ReadAllTextAsync(file, cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            try
            {
                await using (var command = new NpgsqlCommand(sql, connection, transaction))
                {
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                await using (var command = new NpgsqlCommand(
                    "INSERT INTO schema_migrations (script_name, applied_at_utc) VALUES (@scriptName, @appliedAtUtc);",
                    connection,
                    transaction))
                {
                    command.Parameters.AddWithValue("scriptName", migrationName);
                    command.Parameters.AddWithValue("appliedAtUtc", DateTime.UtcNow);
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation("Applied SQL migration {MigrationName}", migrationName);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }

    private string ResolveMigrationsPath()
    {
        if (Path.IsPathRooted(_options.MigrationsPath))
        {
            return _options.MigrationsPath;
        }

        var repoRoot = Directory.GetParent(_hostEnvironment.ContentRootPath)?.Parent?.FullName
            ?? _hostEnvironment.ContentRootPath;

        return Path.GetFullPath(Path.Combine(repoRoot, _options.MigrationsPath));
    }

    private static async Task EnsureMigrationsTableAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS schema_migrations (
                script_name VARCHAR(255) PRIMARY KEY,
                applied_at_utc TIMESTAMPTZ NOT NULL
            );
            """;

        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task<HashSet<string>> LoadAppliedMigrationsAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        const string sql = "SELECT script_name FROM schema_migrations;";

        var applied = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            applied.Add(reader.GetString(0));
        }

        return applied;
    }
}
