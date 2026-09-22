using Auth.Application;
using Auth.Data;
using Auth.Domain;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Presentation;

public static class AuthModuleExtensions
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IAuthAppService, AuthAppService>();
        return services;
    }

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAuth();
        return endpoints;
    }
}
