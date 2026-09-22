using BuildingBlocks.Persistence;
using Categories.Domain;

namespace Categories.Data;

public class CategoryModuleSeeder : IModuleSeeder
{
    private static readonly object _lock = new();

    public int Order => 3;

    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var workId = Guid.Parse("20000000-0000-0000-0000-000000000001");
            if (!context.Categories.Any(c => c.Id == workId))
            {
                context.Categories.AddRange(
                    new CategoryModel
                    {
                        Id = workId,
                        Name = "Work",
                        Description = "Work and professional tasks",
                        Color = "#3B82F6",
                        Icon = "briefcase",
                        CreatedAt = DateTime.UtcNow
                    },
                    new CategoryModel
                    {
                        Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                        Name = "Personal",
                        Description = "Personal errands and daily routine",
                        Color = "#10B981",
                        Icon = "user",
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
