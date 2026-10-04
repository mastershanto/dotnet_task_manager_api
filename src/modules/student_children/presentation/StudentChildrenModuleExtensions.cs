using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Persistence;
using StudentChildren.Domain;
using StudentChildren.Data;
using StudentChildren.Application.Features.StudentChildren;

namespace StudentChildren.Presentation;

public static class StudentChildrenModuleExtensions
{
    public static IServiceCollection AddStudentChildrenModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IStudentChildrenRepository, EfStudentChildrenRepository>();
        services.AddScoped<IModuleSeeder, StudentChildrenModuleSeeder>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetChildrenQuery).Assembly);
        });

        return services;
    }

    public static IEndpointRouteBuilder MapStudentChildrenEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStudentChildren();
        return endpoints;
    }
}
