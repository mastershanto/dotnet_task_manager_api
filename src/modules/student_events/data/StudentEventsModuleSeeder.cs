using BuildingBlocks.Persistence;

namespace StudentEvents.Data;

public class StudentEventsModuleSeeder : IModuleSeeder
{
    public int Order => 80;

    public Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
