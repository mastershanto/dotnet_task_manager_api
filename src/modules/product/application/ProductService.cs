using Products.Domain;
using BuildingBlocks.Abstractions;

namespace Products.Application;

/// <summary>
/// প্রোডাক্ট সার্ভিস ইমপ্লিমেন্টেশন (Product Service Implementation):
/// এটি অ্যাপ্লিকেশন বা বিজনেস লেয়ারের মূল চালিকাশক্তি।
/// কন্ট্রোলার/এন্ডপয়েন্ট থেকে রিকোয়েস্ট আসার পর এটি ভ্যালিডেশন করে এবং
/// ডাটাবেস অপারেশনের জন্য IProductRepository-কে কল করে।
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    /// <summary>
    /// ডিপেনডেন্সি ইনজেকশন (Constructor Injection):
    /// ASP.NET Core-এর DI কন্টেইনার স্বয়ংক্রিয়ভাবে IProductRepository-এর নির্ধারিত
    /// ইমপ্লিমেন্টেশন (যেমন: EfProductRepository) এখানে ইনজেক্ট করে।
    /// </summary>
    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// সমস্ত প্রোডাক্ট রিড করার লজিক:
    /// ডাটাবেস থেকে সব প্রোডাক্ট এনে Success রেজাল্টে মুড়ে পাঠিয়ে দেয়।
    /// </summary>
    public async Task<Result<IEnumerable<ProductModel>>> GetProductsAsync() =>
        Result<IEnumerable<ProductModel>>.Success(await _repo.ListAsync());

    /// <summary>
    /// একক প্রোডাক্ট রিড করার লজিক:
    /// প্রোডাক্ট খুঁজে না পেলে Failure("Not found") পাঠায়, যা প্রেজেন্টেশন লেয়ার 404 হিসেবে হ্যান্ডেল করে।
    /// </summary>
    public async Task<Result<ProductModel>> GetProductAsync(Guid id)
    {
        var product = await _repo.GetAsync(id);
        return product is null
            ? Result<ProductModel>.Failure("Not found")
            : Result<ProductModel>.Success(product);
    }

    /// <summary>
    /// নতুন প্রোডাক্ট তৈরির বিজনেস লজিক:
    /// ১. টাইটেল খালি কি না চেক করে (Business Invariant)।
    /// ২. ভ্যালিড হলে রিপোজিটরি দ্বারা ডাটাবেসে ইনসার্ট করে Success ফেরত দেয়।
    /// </summary>
    public async Task<Result<ProductModel>> CreateProductAsync(ProductModel product)
    {
        if (string.IsNullOrWhiteSpace(product.Title))
            return Result<ProductModel>.Failure("Title required");

        var created = await _repo.CreateAsync(product);
        return Result<ProductModel>.Success(created);
    }

    /// <summary>
    /// প্রোডাক্ট আপডেটের বিজনেস লজিক:
    /// রিপোজিটরির UpdateAsync কল করে। যদি রেকর্ড না থাকে তবে "Not found" Failure পাঠায়।
    /// </summary>
    public async Task<Result<ProductModel>> UpdateProductAsync(Guid id, ProductModel product)
    {
        var updated = await _repo.UpdateAsync(id, product);
        return updated is null
            ? Result<ProductModel>.Failure("Not found")
            : Result<ProductModel>.Success(updated);
    }

    /// <summary>
    /// প্রোডাক্ট ডিলিটের বিজনেস লজিক:
    /// ডিলিট সফল হলে Result<bool>.Success(true) এবং ব্যর্থ হলে Failure পাঠায়।
    /// </summary>
    public async Task<Result<bool>> DeleteProductAsync(Guid id)
    {
        var deleted = await _repo.DeleteAsync(id);
        return deleted
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("Not found");
    }
}
