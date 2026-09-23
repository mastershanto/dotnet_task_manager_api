using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Application;
using Payments.Application.Features.Payments.Commands.ProcessPayment;
using Payments.Data;
using Payments.Domain;

namespace Payments.Presentation;

public static class PaymentsModuleExtensions
{
    public static IServiceCollection AddPaymentsModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Module Repositories & Services
        services.AddScoped<IPaymentService, EfPaymentService>();
        services.AddScoped<IPaymentAppService, PaymentAppService>();

        // 2. CQRS MediatR Handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProcessPaymentCommand).Assembly));

        // 3. Validators
        services.AddValidatorsFromAssembly(typeof(ProcessPaymentCommandValidator).Assembly);

        return services;
    }

    public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPayment();
        return endpoints;
    }
}
