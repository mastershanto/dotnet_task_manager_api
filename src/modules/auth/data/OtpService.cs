using System.Security.Cryptography;
using Auth.Domain;

namespace Auth.Data;

/// <summary>
/// ক্রিপ্টোগ্রাফিকালি সিকিউর ওটিপি জেনারেশন সার্ভিস:
/// ৬-সংখ্যার র্যান্ডম সংখ্যা তৈরি করে।
/// </summary>
public class OtpService : IOtpService
{
    public string GenerateOtp()
    {
        // 100000 থেকে 999999 এর মধ্যে ৬-ডিজিটের ওটিপি
        var otp = RandomNumberGenerator.GetInt32(100_000, 1_000_000);
        return otp.ToString();
    }
}
