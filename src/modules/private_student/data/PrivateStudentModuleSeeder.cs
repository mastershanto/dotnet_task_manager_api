using BuildingBlocks.Persistence;

namespace PrivateStudent.Data;

public class PrivateStudentModuleSeeder : IModuleSeeder
{
    public int Order => 85;

    public Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
