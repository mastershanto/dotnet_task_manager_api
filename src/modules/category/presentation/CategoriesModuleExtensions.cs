using BuildingBlocks.Persistence;
using Categories.Application;
using Categories.Application.Features.Categories.Commands.CreateCategory;
using Categories.Data;
using Categories.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Categories.Presentation;

public static class CategoriesModuleExtensions
{
    public static IServiceCollection AddCategoriesModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Module Repositories
        services.AddScoped<ICategoryRepository, EfCategoryRepository>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IModuleSeeder, CategoryModuleSeeder>();

        // 2. CQRS MediatR Handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateCategoryCommand).Assembly));

        // 3. Validators
        services.AddValidatorsFromAssembly(typeof(CreateCategoryCommandValidator).Assembly);

        return services;
    }

    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCategories();
        return endpoints;
    }
}
