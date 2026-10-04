using BuildingBlocks.Persistence;

namespace Renter.Data;

public class RenterModuleSeeder : IModuleSeeder
{
    public int Order => 100;

    public Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
