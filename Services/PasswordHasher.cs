using System.Security.Cryptography;

namespace TodoApi.Services;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string storedHash);
}

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required", nameof(password));

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

        // Format: v1$iterations$base64(salt)$base64(key)
        return $"v1${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
            return false;

        var parts = storedHash.Split('$', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 4)
            return false;

        if (!string.Equals(parts[0], "v1", StringComparison.Ordinal))
            return false;

        if (!int.TryParse(parts[1], out var iterations))
            return false;

        byte[] salt;
        byte[] expected;

        try
        {
            salt = Convert.FromBase64String(parts[2]);
            expected = Convert.FromBase64String(parts[3]);
        }
        catch
        {
            return false;
        }

        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
