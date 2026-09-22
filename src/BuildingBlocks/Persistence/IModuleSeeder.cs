namespace BuildingBlocks.Persistence;

public interface IModuleSeeder
{
    int Order => 0;
    Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default);
}
