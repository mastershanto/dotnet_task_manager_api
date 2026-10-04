using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Persistence;
using PrivateStudent.Domain;
using PrivateStudent.Data;
using PrivateStudent.Application.Features.PrivateStudent;

namespace PrivateStudent.Presentation;

public static class PrivateStudentModuleExtensions
{
    public static IServiceCollection AddPrivateStudentModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPrivateStudentRepository, EfPrivateStudentRepository>();
        services.AddScoped<IModuleSeeder, PrivateStudentModuleSeeder>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetPrivateClassesQuery).Assembly);
        });

        return services;
    }

    public static IEndpointRouteBuilder MapPrivateStudentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPrivateStudent();
        return endpoints;
    }
}
