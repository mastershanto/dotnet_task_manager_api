using BuildingBlocks.Persistence;

namespace InstructorCalendar.Data;

public class InstructorCalendarModuleSeeder : IModuleSeeder
{
    public int Order => 60;

    public Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
