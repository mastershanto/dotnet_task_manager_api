using App.Features.Auth.Application;
using App.Features.Auth.Data;
using App.Features.Auth.Domain;
using App.Features.Payment.Application;
using App.Features.Payment.Data;
using App.Features.Payment.Domain;
using App.Features.Product.Application;
using App.Features.Product.Data;
using App.Features.Product.Domain;
using App.Features.User.Application;
using App.Features.User.Data;
using App.Features.User.Domain;

namespace Api.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Feature wiring
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IAuthAppService, AuthAppService>();

        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IUserService, UserService>();

        services.AddSingleton<IProductRepository, InMemoryProductRepository>();
        services.AddSingleton<IProductService, ProductService>();

        services.AddSingleton<IPaymentService, PaymentService>();
        services.AddSingleton<IPaymentAppService, PaymentAppService>();

        return services;
    }
}
