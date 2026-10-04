using BuildingBlocks.Persistence;

namespace SystemSettings.Data;

public class SystemSettingsModuleSeeder : IModuleSeeder
{
    public int Order => 10;

    public Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
