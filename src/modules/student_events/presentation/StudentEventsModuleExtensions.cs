using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Persistence;
using StudentEvents.Domain;
using StudentEvents.Data;
using StudentEvents.Application.Features.StudentEvents;

namespace StudentEvents.Presentation;

public static class StudentEventsModuleExtensions
{
    public static IServiceCollection AddStudentEventsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IStudentEventsRepository, EfStudentEventsRepository>();
        services.AddScoped<IModuleSeeder, StudentEventsModuleSeeder>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetStudentEventsQuery).Assembly);
        });

        return services;
    }

    public static IEndpointRouteBuilder MapStudentEventsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStudentEvents();
        return endpoints;
    }
}
