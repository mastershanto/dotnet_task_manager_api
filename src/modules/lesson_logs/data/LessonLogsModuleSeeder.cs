using BuildingBlocks.Persistence;

namespace LessonLogs.Data;

public class LessonLogsModuleSeeder : IModuleSeeder
{
    public int Order => 30;

    public Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
