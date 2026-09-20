using BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;
using Products.Domain;

namespace Products.Data;

/// <summary>
/// ডাটা / ইনফ্রাস্ট্রাকচার লেয়ার (Infrastructure Layer):
/// এটি Entity Framework Core 10 ব্যবহার করে IProductRepository ইন্টারফেসটি বাস্তবায়ন করে।
/// ডাটাবেস কোয়েরি, ট্রানজ্যাকশন ও সেভ করার কাজ এখানে হয়।
/// </summary>
public class EfProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// AppDbContext ইনজেকশন:
    /// ASP.NET Core-এর মাধ্যমে প্রতি HTTP রিকোয়েস্টে একটি Scoped AppDbContext ইন্সট্যান্স সরবরাহ করা হয়।
    /// </summary>
    public EfProductRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// সমস্ত প্রোডাক্ট কুয়েরি:
    /// AsNoTracking(): এটি একটি অত্যন্ত গুরুত্বপূর্ণ অপ্টিমাইজেশন। যেহেতু আমরা ডাটা শুধু পড়ব,
    /// তাই EF Core-কে মেমরিতে চেঞ্জ ট্র্যাক করতে হবে না। ফলে এটি দ্রুত কাজ করে ও মেমরি কম খরচ করে।
    /// OrderBy(x => x.CreatedAt): তৈরির সময় অনুযায়ী সাজায়।
    /// ToListAsync(): ডাটাবেস থেকে নন-ব্লকিং অ্যাসিনক্রোনাস পদ্ধতিতে ডাটা লোড করে।
    /// </summary>
    public async Task<IEnumerable<ProductModel>> ListAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// আইডি দিয়ে একক প্রোডাক্ট খোঁজা:
    /// FirstOrDefaultAsync: ম্যাচিং প্রোডাক্ট পেলে ফেরত দেয়, না পেলে null রিটার্ন করে।
    /// </summary>
    public async Task<ProductModel?> GetAsync(Guid id)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// নতুন প্রোডাক্ট তৈরি ও সেভ করা:
    /// ১. যদি আইডির মান খালি থাকে, নতুন Guid তৈরি করে।
    /// ২. AddAsync দ্বারা চেঞ্জ ট্র্যাকারে যুক্ত করে।
    /// ৩. SaveChangesAsync দ্বারা ডাটাবেসে INSERT কমান্ড এক্সিকিউট করে।
    /// </summary>
    public async Task<ProductModel> CreateAsync(ProductModel product)
    {
        var item = product with
        {
            Id = product.Id == Guid.Empty ? Guid.NewGuid() : product.Id,
            CreatedAt = product.CreatedAt == default ? DateTime.UtcNow : product.CreatedAt
        };

        await _context.Products.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    /// <summary>
    /// বিদ্যমান প্রোডাক্ট আপডেট করা:
    /// ১. FindAsync দিয়ে ডাটাবেসে রেকর্ডটি আছে কি না চেক করে। না থাকলে null ফেরত দেয়।
    /// ২. Detached: পুরনো ট্র্যাকিং অবজেক্ট মুক্ত করে নতুন ভ্যালু সহ আপডেট ট্র্যাকিং শুরু করে।
    /// ৩. Update & SaveChangesAsync দ্বারা ডাটাবেসে UPDATE স্টেটমেন্ট রান করে।
    /// </summary>
    public async Task<ProductModel?> UpdateAsync(Guid id, ProductModel product)
    {
        var existing = await _context.Products.FindAsync(id);
        if (existing is null)
        {
            return null;
        }

        // চেঞ্জ ট্র্যাকার থেকে পুরনো রেফারেন্স বিচ্ছিন্ন করা
        _context.Entry(existing).State = EntityState.Detached;

        // C# রেকর্ড কপি-উইথ সিনট্যাক্স দিয়ে নতুন অবজেক্ট তৈরি
        var updated = existing with
        {
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category
        };

        _context.Products.Update(updated);
        await _context.SaveChangesAsync();
        return updated;
    }

    /// <summary>
    /// প্রোডাক্ট ডিলিট করা:
    /// ১. FindAsync দিয়ে রেকর্ড খুঁজে বের করে।
    /// ২. Remove() মেথডে মুছে ফেলার জন্য মার্ক করে।
    /// ৩. SaveChangesAsync দিয়ে ডাটাবেস থেকে DELETE কুয়েরি এক্সিকিউট করে।
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Products.FindAsync(id);
        if (existing is null)
        {
            return false;
        }

        _context.Products.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
