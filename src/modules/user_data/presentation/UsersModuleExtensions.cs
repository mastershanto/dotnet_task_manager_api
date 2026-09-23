using BuildingBlocks.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application;
using Users.Application.Features.Users.Commands.CreateUser;
using Users.Data;
using Users.Domain;

namespace Users.Presentation;

public static class UsersModuleExtensions
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Module Repositories
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IModuleSeeder, UserModuleSeeder>();

        // 2. CQRS MediatR Handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly));

        // 3. Validators
        services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);

        return services;
    }

    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapUsers();
        return endpoints;
    }
}
