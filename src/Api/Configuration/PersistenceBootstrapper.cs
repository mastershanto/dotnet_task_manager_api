using Api.Infrastructure.Persistence;
using Microsoft.Extensions.Options;

namespace Api.Configuration;

public static class PersistenceBootstrapper
{
    public static async Task ApplyInfrastructureAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        var persistence = app.Services.GetRequiredService<IOptions<PersistenceOptions>>().Value;
        if (!persistence.IsPostgres || !persistence.ApplyMigrationsOnStartup)
        {
            return;
        }

        await using var scope = app.Services.CreateAsyncScope();
        var runner = scope.ServiceProvider.GetRequiredService<PostgresMigrationRunner>();
        await runner.ApplyMigrationsAsync(cancellationToken);
    }
}
