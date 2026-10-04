using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Persistence;
using Notifications.Domain;
using Notifications.Data;
using Notifications.Application.Features.Notifications;

namespace Notifications.Presentation;

public static class NotificationsModuleExtensions
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INotificationsRepository, EfNotificationsRepository>();
        services.AddScoped<IModuleSeeder, NotificationsModuleSeeder>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetNotificationsQuery).Assembly);
        });

        return services;
    }

    public static IEndpointRouteBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapNotifications();
        return endpoints;
    }
}
