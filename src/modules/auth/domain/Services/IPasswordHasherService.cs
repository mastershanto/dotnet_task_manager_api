namespace Auth.Domain;

/// <summary>
/// পাসওয়ার্ড সিকিউরিটি সার্ভিস ইন্টারফেস (PBKDF2 / SHA-256 Hashing)
/// </summary>
public interface IPasswordHasherService
{
    string HashPassword(string plainPassword);
    bool VerifyPassword(string plainPassword, string passwordHash);
}
