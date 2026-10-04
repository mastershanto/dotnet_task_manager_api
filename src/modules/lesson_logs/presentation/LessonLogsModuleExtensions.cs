using BuildingBlocks.Persistence;
using LessonLogs.Application.Features.LessonLogs.Queries.GetLessonLogs;
using LessonLogs.Data;
using LessonLogs.Domain;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LessonLogs.Presentation;

public static class LessonLogsModuleExtensions
{
    public static IServiceCollection AddLessonLogsModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Repository & Seeder
        services.AddSingleton<ILessonLogsRepository, EfLessonLogsRepository>();
        services.AddScoped<IModuleSeeder, LessonLogsModuleSeeder>();

        // 2. CQRS MediatR Handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetLessonLogsQuery).Assembly));

        return services;
    }

    public static IEndpointRouteBuilder MapLessonLogsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapLessonLogs();
        return endpoints;
    }
}
