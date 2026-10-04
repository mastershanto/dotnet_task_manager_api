using BuildingBlocks.Persistence;

namespace InstructorSyllabus.Data;

public class InstructorSyllabusModuleSeeder : IModuleSeeder
{
    public int Order => 50;

    public Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
