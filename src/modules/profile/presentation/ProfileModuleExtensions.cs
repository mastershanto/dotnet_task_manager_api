using BuildingBlocks.Persistence;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Profile.Application.Features.Profiles.Queries.GetProfile;
using Profile.Data;
using Profile.Domain;

namespace Profile.Presentation;

public static class ProfileModuleExtensions
{
    public static IServiceCollection AddProfileModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Repository & Seeder
        services.AddSingleton<IProfileRepository, EfProfileRepository>();
        services.AddScoped<IModuleSeeder, ProfileModuleSeeder>();

        // 2. CQRS MediatR Handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetProfileQuery).Assembly));

        return services;
    }

    public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapProfile();
        return endpoints;
    }
}
