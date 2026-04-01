using App.Features.Auth.Application;
using App.Features.Auth.Data;
using App.Features.Auth.Domain;
using App.Features.Payment.Application;
using App.Features.Payment.Data;
using App.Features.Payment.Domain;
using App.Features.Product.Application;
using App.Features.Product.Data;
using App.Features.Product.Domain;
using App.Features.User.Application;
using App.Features.User.Data;
using App.Features.User.Domain;
using Api.Infrastructure.Persistence;
using Npgsql;

namespace Api.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PersistenceOptions>(configuration.GetSection(PersistenceOptions.SectionName));

        var persistence = configuration.GetSection(PersistenceOptions.SectionName).Get<PersistenceOptions>() ?? new PersistenceOptions();

        if (persistence.IsPostgres)
        {
            var connectionString = configuration.GetConnectionString("Postgres");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'Postgres' is required when Persistence:Provider=Postgres.");

            services.AddSingleton(sp => NpgsqlDataSource.Create(connectionString));
            services.AddSingleton<PostgresMigrationRunner>();

            services.AddSingleton<IUserRepository, PostgresUserRepository>();
            services.AddSingleton<IProductRepository, PostgresProductRepository>();
            services.AddSingleton<IPaymentService, PostgresPaymentService>();
        }
        else
        {
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            services.AddSingleton<IProductRepository, InMemoryProductRepository>();
            services.AddSingleton<IPaymentService, PaymentService>();
        }

        // Feature wiring
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IAuthAppService, AuthAppService>();

        services.AddSingleton<IUserService, UserService>();

        services.AddSingleton<IProductService, ProductService>();

        services.AddSingleton<IPaymentAppService, PaymentAppService>();

        return services;
    }
}
