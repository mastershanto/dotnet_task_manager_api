using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Projects.Application.Features.Projects.Commands.CreateProject;
using Projects.Data;
using Projects.Domain;

namespace Projects.Presentation;

public static class ProjectsModuleExtensions
{
    public static IServiceCollection AddProjectsModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Module Repositories
        services.AddScoped<IProjectRepository, EfProjectRepository>();

        // 2. CQRS MediatR Handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProjectCommand).Assembly));

        // 3. Validators
        services.AddValidatorsFromAssembly(typeof(CreateProjectCommandValidator).Assembly);

        return services;
    }

    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapProjects();
        return endpoints;
    }
}
