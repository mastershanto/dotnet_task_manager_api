using BuildingBlocks.Persistence;

namespace StudentGroupClasses.Data;

public class StudentGroupClassModuleSeeder : IModuleSeeder
{
    public int Order => 70;

    public Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
