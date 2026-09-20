using Auth.Application;
using Auth.Data;
using Auth.Domain;
using BuildingBlocks.Persistence;
using BuildingBlocks.Persistence.Extensions;
using Categories.Application;
using Categories.Data;
using Categories.Domain;
using Payments.Application;
using Payments.Data;
using Payments.Domain;
using Products.Application;
using Products.Data;
using Products.Domain;
using Users.Application;
using Users.Data;
using Users.Domain;
using Api.Infrastructure.Persistence;
using Npgsql;

namespace Api.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PersistenceOptions>(configuration.GetSection(PersistenceOptions.SectionName));

        var persistence = configuration.GetSection(PersistenceOptions.SectionName).Get<PersistenceOptions>() ?? new PersistenceOptions();

        // 1. Enterprise Entity Framework Core Persistence Engine
        services.AddDatabasePersistence(configuration, persistence.IsPostgres);

        if (persistence.IsPostgres)
        {
            var connectionString = configuration.GetConnectionString("Postgres");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'Postgres' is required when Persistence:Provider=Postgres.");

            services.AddSingleton<PostgresMigrationRunner>();
        }

        // 2. Feature Repositories (Powered by EF Core)
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddScoped<ICategoryRepository, EfCategoryRepository>();
        services.AddScoped<IPaymentService, EfPaymentService>();

        // 3. Feature Application Services
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IAuthAppService, AuthAppService>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IPaymentAppService, PaymentAppService>();

        return services;
    }
}
