using BuildingBlocks.Persistence;
using InstructorClasses.Application.Features.InstructorClasses.Queries;
using InstructorClasses.Data;
using InstructorClasses.Domain;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InstructorClasses.Presentation;

public static class InstructorClassesModuleExtensions
{
    public static IServiceCollection AddInstructorClassesModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Repository & Seeder
        services.AddSingleton<IInstructorClassRepository, EfInstructorClassRepository>();
        services.AddScoped<IModuleSeeder, InstructorClassModuleSeeder>();

        // 2. CQRS MediatR Handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetInstructorClassesQuery).Assembly));

        return services;
    }

    public static IEndpointRouteBuilder MapInstructorClassesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapInstructorClasses();
        return endpoints;
    }
}
