using BuildingBlocks.Persistence;
using Products.Domain;

namespace Products.Data;

public class ProductModuleSeeder : IModuleSeeder
{
    private static readonly object _lock = new();

    public int Order => 2;

    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var phoneId = Guid.Parse("10000000-0000-0000-0000-000000000001");
            if (!context.Products.Any(p => p.Id == phoneId))
            {
                context.Products.AddRange(
                    new ProductModel
                    {
                        Id = phoneId,
                        Title = "Smartphone",
                        Description = "4G smartphone",
                        Price = 199.99m,
                        Category = "Electronics",
                        CreatedAt = DateTime.UtcNow
                    },
                    new ProductModel
                    {
                        Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                        Title = "Backpack",
                        Description = "Travel backpack",
                        Price = 49.99m,
                        Category = "Accessories",
                        CreatedAt = DateTime.UtcNow
                    }
                );

                try
                {
                    context.SaveChanges();
                }
                catch (ArgumentException)
                {
                    // Concurrently seeded by another test runner
                }
            }
        }

        await Task.CompletedTask;
    }
}
