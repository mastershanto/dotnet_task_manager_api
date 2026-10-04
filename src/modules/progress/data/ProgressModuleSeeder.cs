using BuildingBlocks.Persistence;

namespace Progress.Data;

public class ProgressModuleSeeder : IModuleSeeder
{
    public int Order => 90;

    public Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
