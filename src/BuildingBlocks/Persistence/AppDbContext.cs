using Auth.Domain;
using Categories.Domain;
using Payments.Domain;
using Products.Domain;
using Projects.Domain;
using Tasks.Domain;
using Users.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BuildingBlocks.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserModel> Users => Set<UserModel>();
    public DbSet<OtpCodeModel> OtpCodes => Set<OtpCodeModel>();
    public DbSet<ProductModel> Products => Set<ProductModel>();
    public DbSet<CategoryModel> Categories => Set<CategoryModel>();
    public DbSet<PaymentModel> Payments => Set<PaymentModel>();
    public DbSet<TaskItemModel> Tasks => Set<TaskItemModel>();
    public DbSet<ProjectModel> Projects => Set<ProjectModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically discover and apply all IEntityTypeConfiguration from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    private static readonly object _seedLock = new();

    private static readonly string DefaultPasswordHash = HashPasswordForSeed("Password123");

    private static string HashPasswordForSeed(string password)
    {
        var salt = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var hash = System.Security.Cryptography.Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            System.Security.Cryptography.HashAlgorithmName.SHA256,
            32);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}:100000";
    }

    public static void SeedData(AppDbContext context)
    {
        lock (_seedLock)
        {
            var adminId = Guid.Parse("00000000-0000-0000-0000-000000000000");
            if (!context.Users.Any(u => u.Email == "admin@example.com"))
            {
                context.Users.Add(
                    new UserModel
                    {
                        Id = adminId,
                        Name = "Admin",
                        Email = "admin@example.com",
                        PasswordHash = DefaultPasswordHash,
                        Role = "admin",
                        IsEmailVerified = true,
                        CreatedAt = DateTime.UtcNow
                    }
                );
            }

            var aliceId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            if (!context.Users.Any(u => u.Id == aliceId))
            {
                context.Users.AddRange(
                    new UserModel
                    {
                        Id = aliceId,
                        Name = "Alice",
                        Email = "alice@example.com",
                        PasswordHash = DefaultPasswordHash,
                        Role = "user",
                        IsEmailVerified = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new UserModel
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                        Name = "Bob",
                        Email = "bob@example.com",
                        PasswordHash = DefaultPasswordHash,
                        Role = "user",
                        IsEmailVerified = true,
                        CreatedAt = DateTime.UtcNow
                    }
                );
            }

            var phoneId = Guid.Parse("10000000-0000-0000-0000-000000000001");
            if (!context.Products.Any(p => p.Id == phoneId))
            {
                context.Products.AddRange(
                    new ProductModel
                    {
                        Id = phoneId,
                        Title = "Smartphone",
                        Description = "4G smartphone",
                        Price = 199.99m,
                        Category = "Electronics",
                        CreatedAt = DateTime.UtcNow
                    },
                    new ProductModel
                    {
                        Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                        Title = "Backpack",
                        Description = "Travel backpack",
                        Price = 49.99m,
                        Category = "Accessories",
                        CreatedAt = DateTime.UtcNow
                    }
                );
            }

            var workId = Guid.Parse("20000000-0000-0000-0000-000000000001");
            if (!context.Categories.Any(c => c.Id == workId))
            {
                context.Categories.AddRange(
                    new CategoryModel
                    {
                        Id = workId,
                        Name = "Work",
                        Description = "Work and professional tasks",
                        Color = "#3B82F6",
                        Icon = "briefcase",
                        CreatedAt = DateTime.UtcNow
                    },
                    new CategoryModel
                    {
                        Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                        Name = "Personal",
                        Description = "Personal errands and daily routine",
                        Color = "#10B981",
                        Icon = "user",
                        CreatedAt = DateTime.UtcNow
                    }
                );
            }

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
}
