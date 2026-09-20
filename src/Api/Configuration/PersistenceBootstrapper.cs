using Api.Infrastructure.Persistence;
using BuildingBlocks.Persistence;
using Microsoft.Extensions.Options;

namespace Api.Configuration;

public static class PersistenceBootstrapper
{
    public static async Task ApplyInfrastructureAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var persistence = scope.ServiceProvider.GetRequiredService<IOptions<PersistenceOptions>>().Value;

        if (persistence.IsPostgres && persistence.ApplyMigrationsOnStartup)
        {
            var runner = scope.ServiceProvider.GetRequiredService<PostgresMigrationRunner>();
            await runner.ApplyMigrationsAsync(cancellationToken);
        }
        else if (!persistence.IsPostgres)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            AppDbContext.SeedData(dbContext);
        }
    }
}
