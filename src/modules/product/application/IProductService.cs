using Products.Domain;
using BuildingBlocks.Abstractions;

namespace Products.Application;

/// <summary>
/// অ্যাপ্লিকেশন সার্ভিস ইন্টারফেস (Application Service Interface):
/// এটি বিজনেস লজিক বা ইউজ-কেস (Use Case) লেয়ারের চুক্তিপত্র।
/// সাধারণ ডাটা টাইপের পরিবর্তে 'Result<T>' প্যাটার্ন ব্যবহার করা হয়েছে,
/// যাতে কোনো এরর ঘটলে এক্সেপশন না ছুড়ে প্রিডিক্টেবল ও ক্লিন রেসপন্স দেওয়া যায়।
/// </summary>
public interface IProductService
{
    /// <summary>
    /// সমস্ত প্রোডাক্টের তালিকা রিটার্ন করে। সফল হলে Result.Success(তালিকা) ফেরত আসে।
    /// </summary>
    Task<Result<IEnumerable<ProductModel>>> GetProductsAsync();

    /// <summary>
    /// নির্দিষ্ট আইডির প্রোডাক্টের তথ্য নিয়ে আসে। প্রোডাক্ট না থাকলে Failure("Not found") ফেরত দেয়।
    /// </summary>
    Task<Result<ProductModel>> GetProductAsync(Guid id);

    /// <summary>
    /// নতুন প্রোডাক্ট তৈরির বিজনেস লজিক এক্সিকিউট করে। টাইটেল খালি থাকলে Failure ফেরত দেয়।
    /// </summary>
    Task<Result<ProductModel>> CreateProductAsync(ProductModel product);

    /// <summary>
    /// বিদ্যমান প্রোডাক্টের তথ্য আপডেট করে।
    /// </summary>
    Task<Result<ProductModel>> UpdateProductAsync(Guid id, ProductModel product);

    /// <summary>
    /// প্রোডাক্ট ডিলিট করার বিজনেস লজিক এক্সিকিউট করে।
    /// </summary>
    Task<Result<bool>> DeleteProductAsync(Guid id);
}
