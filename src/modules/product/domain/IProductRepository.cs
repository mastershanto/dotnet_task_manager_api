namespace Products.Domain;

/// <summary>
/// রিপোজিটরি ইন্টারফেস (Repository Interface):
/// ডিপেনডেন্সি ইনভার্সন প্রিন্সিপল (DIP) অনুযায়ী ডোমেন লেয়ার ঠিক করে ডাটাবেসে কী কী অপারেশন চালানো যাবে।
/// কিন্তু কীভাবে ডাটা সেভ বা ফেচ করা হবে (যেমন: EF Core নাকি Dapper নাকি InMemory) তা এই লেয়ার জানে না।
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// সমস্ত প্রোডাক্টের তালিকা অ্যাসিঙ্ক্রোনাসভাবে ডাটাবেস থেকে তুলে আনে।
    /// </summary>
    Task<IEnumerable<ProductModel>> ListAsync();

    /// <summary>
    /// নির্দিষ্ট আইডি (Id) দিয়ে একটি একক প্রোডাক্ট খুঁজে বের করে।
    /// প্রোডাক্ট না পাওয়া গেলে নাল (null) রিটার্ন করে।
    /// </summary>
    Task<ProductModel?> GetAsync(Guid id);

    /// <summary>
    /// একটি নতুন প্রোডাক্ট ডাটাবেসে সেভ করে এবং সেভ হওয়া অবজেক্টটি ফেরত দেয়।
    /// </summary>
    Task<ProductModel> CreateAsync(ProductModel product);

    /// <summary>
    /// নির্দিষ্ট আইডির প্রোডাক্টের তথ্য আপডেট করে।
    /// যদি প্রোডাক্টটি না পাওয়া যায়, তবে নাল (null) রিটার্ন করে।
    /// </summary>
    Task<ProductModel?> UpdateAsync(Guid id, ProductModel product);

    /// <summary>
    /// নির্দিষ্ট আইডির প্রোডাক্ট ডাটাবেস থেকে ডিলিট করে।
    /// সফলভাবে ডিলিট হলে true এবং না পাওয়া গেলে false রিটার্ন করে।
    /// </summary>
    Task<bool> DeleteAsync(Guid id);
}
