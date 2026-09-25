using BuildingBlocks.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tasks.Application.Features.Tasks.Commands.CreateTask;
using Tasks.Data;
using Tasks.Domain;

namespace Tasks.Presentation;

public static class TasksModuleExtensions
{
    public static IServiceCollection AddTasksModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Module Repositories & Seeders
        services.AddScoped<ITaskRepository, EfTaskRepository>();
        services.AddScoped<IModuleSeeder, TaskModuleSeeder>();

        // 2. CQRS MediatR Handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTaskCommand).Assembly));

        // 3. Validators
        services.AddValidatorsFromAssembly(typeof(CreateTaskCommandValidator).Assembly);

        return services;
    }

    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapTasks();
        return endpoints;
    }
}
