using BuildingBlocks.Persistence;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Products.Application;
using Products.Data;
using Products.Domain;

namespace Products.Presentation;

public static class ProductsModuleExtensions
{
    public static IServiceCollection AddProductsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IModuleSeeder, ProductModuleSeeder>();
        return services;
    }

    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapProducts();
        return endpoints;
    }
}
