using BuildingBlocks.Persistence;
using Categories.Application;
using Categories.Data;
using Categories.Domain;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Categories.Presentation;

public static class CategoriesModuleExtensions
{
    public static IServiceCollection AddCategoriesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICategoryRepository, EfCategoryRepository>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IModuleSeeder, CategoryModuleSeeder>();
        return services;
    }

    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCategories();
        return endpoints;
    }
}
