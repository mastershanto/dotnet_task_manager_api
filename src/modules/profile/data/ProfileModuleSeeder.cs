using BuildingBlocks.Persistence;

namespace Profile.Data;

public class ProfileModuleSeeder : IModuleSeeder
{
    public int Order => 20;

    public Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
