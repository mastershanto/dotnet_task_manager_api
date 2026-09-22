using Api.Infrastructure.Persistence;
using Auth.Presentation;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Persistence;
using BuildingBlocks.Persistence.Extensions;
using Categories.Presentation;
using MediatR;
using Payments.Presentation;
using Products.Presentation;
using Projects.Presentation;
using Tasks.Presentation;
using Users.Presentation;

namespace Api.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Enterprise Persistence Configuration
        services.Configure<PersistenceOptions>(configuration.GetSection(PersistenceOptions.SectionName));
        var persistence = configuration.GetSection(PersistenceOptions.SectionName).Get<PersistenceOptions>() ?? new PersistenceOptions();

        services.AddDatabasePersistence(configuration, persistence.IsPostgres);

        if (persistence.IsPostgres)
        {
            var connectionString = configuration.GetConnectionString("Postgres");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'Postgres' is required when Persistence:Provider=Postgres.");

            services.AddSingleton<PostgresMigrationRunner>();
        }

        // 2. Cross-Cutting Pipeline Behaviors (MediatR)
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // 3. Decentralized Module Services (Each module manages its own dependencies)
        services.AddAuthModule(configuration);
        services.AddUsersModule(configuration);
        services.AddProductsModule(configuration);
        services.AddCategoriesModule(configuration);
        services.AddPaymentsModule(configuration);
        services.AddTasksModule(configuration);
        services.AddProjectsModule(configuration);

        return services;
    }
}
