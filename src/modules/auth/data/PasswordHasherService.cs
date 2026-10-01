using System.Security.Cryptography;
using Auth.Domain;

namespace Auth.Data;

/// <summary>
/// ইন্ডাস্ট্রি স্ট্যান্ডার্ড পাসওয়ার্ড হ্যাশিং সার্ভিস:
/// PBKDF2 (Password-Based Key Derivation Function 2) অ্যালগরিদম এবং HMAC-SHA256 ব্যবহার করে
/// ১২৮-বিট ক্রিপ্টোগ্রাফিক সল্ট (Salt) সহ পাসওয়ার্ড হ্যাশ করে।
/// </summary>
public class PasswordHasherService : IPasswordHasherService
{
    private const int SaltSize = 16; // 128 bit
    private const int KeySize = 32;  // 256 bit
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;
    private const char SegmentDelimiter = ':';

    public string HashPassword(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Password cannot be empty.", nameof(plainPassword));

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            plainPassword,
            salt,
            Iterations,
            HashAlgorithm,
            KeySize);

        return string.Join(
            SegmentDelimiter,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash),
            Iterations);
    }

    public bool VerifyPassword(string plainPassword, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(plainPassword) || string.IsNullOrWhiteSpace(passwordHash))
            return false;

        var segments = passwordHash.Split(SegmentDelimiter);
        if (segments.Length != 3)
            return false;

        var salt = Convert.FromBase64String(segments[0]);
        var expectedHash = Convert.FromBase64String(segments[1]);
        if (!int.TryParse(segments[2], out var iterations))
            return false;

        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            plainPassword,
            salt,
            iterations,
            HashAlgorithm,
            expectedHash.Length);

        // Timing-attack safe comparison (CryptographicOperations.FixedTimeEquals)
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
