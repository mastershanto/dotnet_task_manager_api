using BuildingBlocks.Persistence;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application;
using Users.Data;
using Users.Domain;

namespace Users.Presentation;

public static class UsersModuleExtensions
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IModuleSeeder, UserModuleSeeder>();
        return services;
    }

    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapUsers();
        return endpoints;
    }
}
