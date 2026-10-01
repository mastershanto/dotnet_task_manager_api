namespace Auth.Domain;

/// <summary>
/// ওটিপি ব্যবহারের উদ্দেশ্য (OTP Purpose):
/// Registration: সাইনআপের ইমেইল ভেরিফিকেশন
/// PasswordReset: পাসওয়ার্ড ভুলে গেলে রিসেট ওটিপি
/// </summary>
public enum OtpPurpose
{
    Registration = 1,
    PasswordReset = 2
}

/// <summary>
/// ওটিপি কোড ডোমেন মডেল (OTP Code Entity):
/// ৬-সংখ্যার ওটিপি কোড ও তার মেয়াদ সংক্রান্ত তথ্য সংরক্ষণ করে।
/// </summary>
public record OtpCodeModel
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Email { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;

    public OtpPurpose Purpose { get; init; } = OtpPurpose.Registration;

    public DateTime ExpiresAt { get; init; }

    public bool IsUsed { get; init; } = false;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
