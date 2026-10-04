using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Persistence;
using InstructorCalendar.Domain;
using InstructorCalendar.Data;
using InstructorCalendar.Application.Features.InstructorCalendar.Queries;

namespace InstructorCalendar.Presentation;

public static class InstructorCalendarModuleExtensions
{
    public static IServiceCollection AddInstructorCalendarModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IInstructorCalendarRepository, EfInstructorCalendarRepository>();
        services.AddScoped<IModuleSeeder, InstructorCalendarModuleSeeder>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetCalendarEventsQuery).Assembly);
        });

        return services;
    }

    public static IEndpointRouteBuilder MapInstructorCalendarEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapInstructorCalendar();
        return endpoints;
    }
}
