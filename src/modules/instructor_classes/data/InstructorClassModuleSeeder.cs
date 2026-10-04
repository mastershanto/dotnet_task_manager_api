using BuildingBlocks.Persistence;

namespace InstructorClasses.Data;

public class InstructorClassModuleSeeder : IModuleSeeder
{
    public int Order => 40;

    public Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
