using Auth.Application;
using Auth.Application.Features.Auth.Commands.Register;
using Auth.Data;
using Auth.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Presentation;

public static class AuthModuleExtensions
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Repositories & Security Services
        services.AddScoped<IAuthRepository, EfAuthRepository>();
        services.AddSingleton<IPasswordHasherService, PasswordHasherService>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<IOtpService, OtpService>();

        // 2. CQRS MediatR Handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly));

        // 3. Validators
        services.AddValidatorsFromAssembly(typeof(RegisterCommandValidator).Assembly);

        return services;
    }

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAuth();
        return endpoints;
    }
}
