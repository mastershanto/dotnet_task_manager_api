using Categories.Domain;
using Payments.Domain;
using Products.Domain;
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
    public DbSet<ProductModel> Products => Set<ProductModel>();
    public DbSet<CategoryModel> Categories => Set<CategoryModel>();
    public DbSet<PaymentModel> Payments => Set<PaymentModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply any entity configurations from the calling/loaded assemblies
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // 1. Users Mapping
        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        });

        // 2. Products Mapping
        modelBuilder.Entity<ProductModel>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(150).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
            entity.Property(e => e.Price).HasColumnName("price").HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.Category).HasColumnName("category").HasMaxLength(100).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        });

        // 3. Categories Mapping
        modelBuilder.Entity<CategoryModel>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.Color).HasColumnName("color").HasMaxLength(50);
            entity.Property(e => e.Icon).HasColumnName("icon").HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        });

        // 4. Payments Mapping
        modelBuilder.Entity<PaymentModel>(entity =>
        {
            entity.ToTable("payments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.Currency).HasColumnName("currency").HasMaxLength(10).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.HasIndex(e => e.UserId);
        });
    }

    private static readonly object _seedLock = new();

    public static void SeedData(AppDbContext context)
    {
        lock (_seedLock)
        {
            var aliceId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            if (!context.Users.Any(u => u.Id == aliceId))
            {
                context.Users.AddRange(
                    new UserModel
                    {
                        Id = aliceId,
                        Name = "Alice",
                        Email = "alice@example.com",
                        CreatedAt = DateTime.UtcNow
                    },
                    new UserModel
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                        Name = "Bob",
                        Email = "bob@example.com",
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
