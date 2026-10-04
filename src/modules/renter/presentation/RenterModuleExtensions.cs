using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Persistence;
using Renter.Domain;
using Renter.Data;
using Renter.Application.Features.Renter;

namespace Renter.Presentation;

public static class RenterModuleExtensions
{
    public static IServiceCollection AddRenterModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRenterRepository, EfRenterRepository>();
        services.AddScoped<IModuleSeeder, RenterModuleSeeder>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetRenterCitiesQuery).Assembly);
        });

        return services;
    }

    public static IEndpointRouteBuilder MapRenterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapRenter();
        return endpoints;
    }
}
