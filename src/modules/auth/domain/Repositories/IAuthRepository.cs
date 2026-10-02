using Users.Domain;

namespace Auth.Domain;

/// <summary>
/// অথেন্টিকেশন রিপোজিটরি ইন্টারফেস (Repository Pattern Contract):
/// ডাটাবেস অপারেশনগুলোর স্পেসিফিকেশন নির্ধারণ করে।
/// Clean Architecture অনুযায়ী Application Layer শুধুমাত্র এই ইন্টারফেসের ওপর নির্ভর করে।
/// </summary>
public interface IAuthRepository
{
    Task<UserModel?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserModel?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserModel> CreateUserAsync(UserModel user, CancellationToken cancellationToken = default);
    Task<UserModel> UpdateUserAsync(UserModel user, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);

    Task SaveOtpAsync(OtpCodeModel otp, CancellationToken cancellationToken = default);
    Task<OtpCodeModel?> GetActiveOtpAsync(string email, string code, OtpPurpose purpose, CancellationToken cancellationToken = default);
    Task MarkOtpUsedAsync(Guid otpId, CancellationToken cancellationToken = default);
    Task InvalidatePreviousOtpsAsync(string email, OtpPurpose purpose, CancellationToken cancellationToken = default);
}
