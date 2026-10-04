using BuildingBlocks.Persistence;

namespace StudentChildren.Data;

public class StudentChildrenModuleSeeder : IModuleSeeder
{
    public int Order => 75;

    public Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
