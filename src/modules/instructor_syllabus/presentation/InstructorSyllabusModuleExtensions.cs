using BuildingBlocks.Persistence;
using InstructorSyllabus.Application.Features.InstructorSyllabus.Queries;
using InstructorSyllabus.Data;
using InstructorSyllabus.Domain;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InstructorSyllabus.Presentation;

public static class InstructorSyllabusModuleExtensions
{
    public static IServiceCollection AddInstructorSyllabusModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Repository & Seeder
        services.AddSingleton<IInstructorSyllabusRepository, EfInstructorSyllabusRepository>();
        services.AddScoped<IModuleSeeder, InstructorSyllabusModuleSeeder>();

        // 2. CQRS MediatR Handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetInstructorSyllabiQuery).Assembly));

        return services;
    }

    public static IEndpointRouteBuilder MapInstructorSyllabusEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapInstructorSyllabus();
        return endpoints;
    }
}
