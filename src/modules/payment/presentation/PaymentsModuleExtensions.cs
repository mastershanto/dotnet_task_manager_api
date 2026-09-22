using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Application;
using Payments.Data;
using Payments.Domain;

namespace Payments.Presentation;

public static class PaymentsModuleExtensions
{
    public static IServiceCollection AddPaymentsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPaymentService, EfPaymentService>();
        services.AddScoped<IPaymentAppService, PaymentAppService>();
        return services;
    }

    public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPayment();
        return endpoints;
    }
}
