using Users.Domain;

namespace Auth.Domain;

/// <summary>
/// JWT টোকেন জেনারেটর সার্ভিস ইন্টারফেস
/// </summary>
public interface ITokenService
{
    string GenerateToken(UserModel user);
}
