using BuildingBlocks.Persistence;
using Users.Domain;

namespace Users.Data;

public class UserModuleSeeder : IModuleSeeder
{
    private static readonly object _lock = new();

    public int Order => 1;

    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var aliceId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            if (!context.Users.Any(u => u.Id == aliceId))
            {
                context.Users.AddRange(
                    new UserModel
                    {
                        Id = aliceId,
                        Name = "Alice",
                        Email = "alice@example.com",
                        CreatedAt = DateTime.UtcNow
                    },
                    new UserModel
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                        Name = "Bob",
                        Email = "bob@example.com",
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
