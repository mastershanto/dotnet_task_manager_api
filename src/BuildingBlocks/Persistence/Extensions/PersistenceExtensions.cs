using BuildingBlocks.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddDatabasePersistence(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isPostgres)
    {
        services.AddSingleton<AuditableEntityInterceptor>();

        if (isPostgres)
        {
            var connectionString = configuration.GetConnectionString("Postgres");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'Postgres' is required when Persistence:Provider=Postgres.");

            services.AddDbContextPool<AppDbContext>((sp, options) =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null);
                    npgsqlOptions.CommandTimeout(30);
                });
                options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
            });
        }
        else
        {
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("TaskManagerDb");
                options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
            });
        }

        return services;
    }
}
