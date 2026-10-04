using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Persistence;
using Progress.Domain;
using Progress.Data;
using Progress.Application.Features.Progress;

namespace Progress.Presentation;

public static class ProgressModuleExtensions
{
    public static IServiceCollection AddProgressModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProgressRepository, EfProgressRepository>();
        services.AddScoped<IModuleSeeder, ProgressModuleSeeder>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetStudentProgressQuery).Assembly);
        });

        return services;
    }

    public static IEndpointRouteBuilder MapProgressEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapProgress();
        return endpoints;
    }
}
