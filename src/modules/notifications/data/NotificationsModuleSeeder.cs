using BuildingBlocks.Persistence;

namespace Notifications.Data;

public class NotificationsModuleSeeder : IModuleSeeder
{
    public int Order => 95;

    public Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
