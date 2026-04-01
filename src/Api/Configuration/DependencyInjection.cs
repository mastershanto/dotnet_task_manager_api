using Auth.Application;
using Auth.Data;
using Auth.Domain;
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
