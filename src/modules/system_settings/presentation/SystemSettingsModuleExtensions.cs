using BuildingBlocks.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SystemSettings.Application.Features.SystemSettings.Queries.GetSystemSettings;
using SystemSettings.Data;
using SystemSettings.Domain;

namespace SystemSettings.Presentation;

public static class SystemSettingsModuleExtensions
{
    public static IServiceCollection AddSystemSettingsModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Repository & Seeder
        services.AddSingleton<ISystemSettingsRepository, EfSystemSettingsRepository>();
        services.AddScoped<IModuleSeeder, SystemSettingsModuleSeeder>();

        // 2. CQRS MediatR Handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetSystemSettingsQuery).Assembly));

        return services;
    }

    public static IEndpointRouteBuilder MapSystemSettingsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapSystemSettings();
        return endpoints;
    }
}
