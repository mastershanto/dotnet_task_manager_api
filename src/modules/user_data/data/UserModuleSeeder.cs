using System.Security.Cryptography;
using BuildingBlocks.Persistence;
using Users.Domain;

namespace Users.Data;

public class UserModuleSeeder : IModuleSeeder
{
    private static readonly object _lock = new();

    public int Order => 1;

    private static readonly string DefaultPasswordHash = HashPasswordForSeed("Password123");

    private static string HashPasswordForSeed(string password)
    {
        var salt = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}:100000";
    }

    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        lock (_lock)
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

            try
            {
                context.SaveChanges();
            }
            catch (Exception)
            {
                // Concurrently seeded by another test runner or unique constraint
            }
        }

        await Task.CompletedTask;
    }
}
