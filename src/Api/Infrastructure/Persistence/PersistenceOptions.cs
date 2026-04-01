namespace Api.Infrastructure.Persistence;

public sealed class PersistenceOptions
{
    public const string SectionName = "Persistence";

    public string Provider { get; set; } = "InMemory";
    public bool ApplyMigrationsOnStartup { get; set; } = true;
    public string MigrationsPath { get; set; } = "infra/postgres/migrations";

    public bool IsPostgres =>
        Provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase);
}
