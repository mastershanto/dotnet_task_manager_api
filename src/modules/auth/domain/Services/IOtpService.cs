namespace Auth.Domain;

/// <summary>
/// ওটিপি জেনারেশন ও ভ্যালিডেশন সার্ভিস ইন্টারফেস
/// </summary>
public interface IOtpService
{
    string GenerateOtp();
}
