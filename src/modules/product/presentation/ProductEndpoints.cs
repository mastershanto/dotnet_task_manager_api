using Products.Application;
using Products.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Products.Presentation;

/// <summary>
/// প্রেজেন্টেশন লেয়ার (Presentation Layer - Minimal APIs):
/// এটি প্রোডাক্ট ফিচারের সমস্ত HTTP এন্ডপয়েন্ট এক্সপোজ করে।
/// ট্র্যাডিশনাল MVC কন্ট্রোলারের চেয়ে Minimal APIs অনেক দ্রুত এবং হালকা।
/// </summary>
public static class ProductEndpoints
{
    /// <summary>
    /// প্রোডাক্ট সংক্রান্ত সমস্ত রুট ম্যাপিং মেথড:
    /// </summary>
    public static void MapProducts(this IEndpointRouteBuilder endpoints)
    {
        // /products রুটের জন্য একটি গ্রুপ তৈরি করা হচ্ছে
        // WithTags("Products"): Swagger UI-তে সুন্দরভাবে "Products" ট্যাগের আন্ডারে দেখানোর জন্য
        // RequireAuthorization(AuthPolicies.ApiUser): শুধুমাত্র ভ্যালিড JWT লগইন করা ইউজাররা এই API ব্যবহার করতে পারবে
        var group = endpoints.MapGroup("/products")
            .WithTags("Products")
            .RequireAuthorization(AuthPolicies.ApiUser);

        // -------------------------------------------------------------
        // ১. GET /products (সমস্ত প্রোডাক্টের তালিকা আনা)
        // -------------------------------------------------------------
        group.MapGet("/", async (IProductService productService) =>
        {
            var result = await productService.GetProductsAsync();
            // Result.IsSuccess ট্রু হলে HTTP 200 OK ডাটা সহ পাঠায়, অন্যথায় ইন্টারনাল সার্ভার এরর পাঠায়
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(string.Join(",", result.Errors));
        })
        .Produces<IEnumerable<ProductModel>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        // -------------------------------------------------------------
        // ২. GET /products/{id} (নির্দিষ্ট আইডির প্রোডাক্ট দেখা)
        // -------------------------------------------------------------
        group.MapGet("/{id:guid}", async (IProductService productService, Guid id) =>
        {
            var result = await productService.GetProductAsync(id);
            // প্রোডাক্ট পাওয়া গেলে 200 OK, না পাওয়া গেলে 404 Not Found রেসপন্স পাঠায়
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result.Errors);
        })
        .Produces<ProductModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // -------------------------------------------------------------
        // ৩. POST /products (নতুন প্রোডাক্ট তৈরি করা)
        // -------------------------------------------------------------
        group.MapPost("/", async (IProductService productService, ProductModel product) =>
        {
            // মডেল ভ্যালিডেশন চেক (Data Annotations যেমন Required, StringLength ইত্যাদি)
            var validation = Validation.Validate(product).ToArray();
            if (validation.Any())
                return Results.ValidationProblem(Validation.ToErrorDictionary(validation));

            var result = await productService.CreateProductAsync(product);
            // সফল হলে HTTP 201 Created রেসপন্স ও লোকেশন হেডার সহ অবজেক্টটি ফেরত দেওয়া হয়
            return result.IsSuccess
                ? Results.Created($"/products/{result.Value!.Id}", result.Value)
                : Results.BadRequest(result.Errors);
        })
        .Produces<ProductModel>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status400BadRequest);

        // -------------------------------------------------------------
        // ৪. PUT /products/{id} (বিদ্যমান প্রোডাক্টের তথ্য আপডেট করা)
        // -------------------------------------------------------------
        group.MapPut("/{id:guid}", async (IProductService productService, Guid id, ProductModel product) =>
        {
            var validation = Validation.Validate(product).ToArray();
            if (validation.Any())
                return Results.ValidationProblem(Validation.ToErrorDictionary(validation));

            var result = await productService.UpdateProductAsync(id, product);
            // আপডেট সফল হলে 200 OK, না পাওয়া গেলে 404 Not Found
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result.Errors);
        })
        .Produces<ProductModel>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound);

        // -------------------------------------------------------------
        // ৫. DELETE /products/{id} (প্রোডাক্ট ডিলিট করা)
        // -------------------------------------------------------------
        group.MapDelete("/{id:guid}", async (IProductService productService, Guid id) =>
        {
            var result = await productService.DeleteProductAsync(id);
            // ডিলিট সফল হলে HTTP 204 No Content (বডি ছাড়া সাকসেস) ফেরত যায়
            return result.IsSuccess
                ? Results.NoContent()
                : Results.NotFound(result.Errors);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
