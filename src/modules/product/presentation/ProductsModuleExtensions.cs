using BuildingBlocks.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Products.Application;
using Products.Application.Features.Products.Commands.CreateProduct;
using Products.Data;
using Products.Domain;

namespace Products.Presentation;

public static class ProductsModuleExtensions
{
    public static IServiceCollection AddProductsModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Module Repositories
        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IModuleSeeder, ProductModuleSeeder>();

        // 2. CQRS MediatR Handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));

        // 3. Validators
        services.AddValidatorsFromAssembly(typeof(CreateProductCommandValidator).Assembly);

        return services;
    }

    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapProducts();
        return endpoints;
    }
}
