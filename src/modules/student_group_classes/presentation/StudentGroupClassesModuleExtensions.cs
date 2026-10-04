using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Persistence;
using StudentGroupClasses.Domain;
using StudentGroupClasses.Data;
using StudentGroupClasses.Application.Features.StudentGroupClasses.Queries;

namespace StudentGroupClasses.Presentation;

public static class StudentGroupClassesModuleExtensions
{
    public static IServiceCollection AddStudentGroupClassesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IStudentGroupClassRepository, EfStudentGroupClassRepository>();
        services.AddScoped<IModuleSeeder, StudentGroupClassModuleSeeder>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetStudentGroupClassesQuery).Assembly);
        });

        return services;
    }

    public static IEndpointRouteBuilder MapStudentGroupClassesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStudentGroupClasses();
        return endpoints;
    }
}
