using System.ComponentModel.DataAnnotations;

namespace Products.Domain;

/// <summary>
/// ডোমেন মডেল (Domain Model):
/// এটি হলো প্রোডাক্টের কোর বিজনেস এন্টিটি (Pure POCO)।
/// এতে কোনো ডাটাবেস ফ্রেমওয়ার্কের (যেমন: EF Core বা SQL) সরাসরি ডিপেনডেন্সি নেই।
/// 'record' ব্যবহারের কারণে এটি অপরিবর্তনশীল (Immutable) এবং থ্রেড-সেফ।
/// </summary>
public record ProductModel
{
    /// <summary>
    /// প্রোডাক্টের ইউনিক আইডেন্টিফায়ার (Primary Key)।
    /// ডিফল্টভাবে একটি নতুন GUID (Globally Unique Identifier) অ্যাসাইন হয়।
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// প্রোডাক্টের নাম বা শিরোনাম।
    /// [Required]: ফিল্ডটি অবশ্যই পূরণ করতে হবে (নাল বা খালি রাখা যাবে না)।
    /// [StringLength(150, MinimumLength = 2)]: নামের দৈর্ঘ্য সর্বনিম্ন ২ এবং সর্বোচ্চ ১৫০ অক্ষর হতে হবে।
    /// </summary>
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// প্রোডাক্টের বিস্তারিত বিবরণ।
    /// [Required]: বিবরণ দেওয়া বাধ্যতামূলক।
    /// [StringLength(500)]: সর্বোচ্চ ৫০০ অক্ষরের মধ্যে সীমাবদ্ধ।
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// প্রোডাক্টের মূল্য (Price)।
    /// [Range(0.01, double.MaxValue)]: মূল্য অবশ্যই ০.০১ বা তার বেশি পজিটিভ সংখ্যা হতে হবে।
    /// </summary>
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; init; }

    /// <summary>
    /// প্রোডাক্টটি কোন ক্যাটাগরির (যেমন: Electronics, Clothing ইত্যাদি)।
    /// [Required]: ক্যাটাগরি উল্লেখ করা বাধ্যতামূলক।
    /// </summary>
    [Required]
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// প্রোডাক্টটি তৈরির সময় (UTC টাইমজোনে)।
    /// ডিফল্টভাবে বর্তমান সার্বজনীন সময় (DateTime.UtcNow) ধারণ করে।
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
