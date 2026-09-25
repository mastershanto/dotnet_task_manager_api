using BuildingBlocks.Persistence;
using Tasks.Domain;

namespace Tasks.Data;

public class TaskModuleSeeder : IModuleSeeder
{
    private static readonly object _lock = new();

    public int Order => 5;

    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var taskId = Guid.Parse("30000000-0000-0000-0000-000000000001");
            if (!context.Tasks.Any(t => t.Id == taskId))
            {
                context.Tasks.AddRange(
                    new TaskItemModel
                    {
                        Id = taskId,
                        Title = "Initial Project Setup",
                        Description = "Configure clean architecture and CQRS patterns",
                        Status = TaskItemStatus.Completed,
                        Priority = TaskPriority.High,
                        CategoryId = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                        CreatedAt = DateTime.UtcNow
                    }
                );

                try
                {
                    context.SaveChanges();
                }
                catch (ArgumentException)
                {
                    // Concurrently seeded by another test runner
                }
            }
        }

        await Task.CompletedTask;
    }
}
