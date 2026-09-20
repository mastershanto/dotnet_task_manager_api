using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Products.Domain;

namespace BuildingBlocks.Persistence.Configurations;

/// <summary>
/// ডাটাবেস স্কিমা কনফিগারেশন (Fluent API Configuration):
/// Entity Framework Core-এ ডোমেন মডেলের প্রপার্টিগুলো ডাটাবেস টেবিলের কোন কলামে কেমন টাইপে বসবে,
/// তা ডোমেন ক্লাসে না লিখে এই আলাদা কনফিগারেশন ক্লাসে পরিষ্কারভাবে নির্ধারণ করা হয়।
/// এটি Clean Architecture-এর মূল নীতি মেনে চলে (Domain layer blijft 100% pure)।
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<ProductModel>
{
    public void Configure(EntityTypeBuilder<ProductModel> builder)
    {
        // ডাটাবেসে টেবিলের নাম নির্ধারণ করা হলো 'products'
        builder.ToTable("products");

        // Id প্রপার্টিটিকে Primary Key হিসেবে নির্ধারণ করা হলো
        builder.HasKey(p => p.Id);

        // কলামের নাম ও ডাটাবেস কনস্ট্রেইন্ট ম্যাপিং:
        builder.Property(p => p.Id).HasColumnName("id");

        // Title: সর্বোচ্চ ১৫০ অক্ষর এবং এটি বাধ্যতামূলক (NOT NULL)
        builder.Property(p => p.Title)
            .HasColumnName("title")
            .HasMaxLength(150)
            .IsRequired();

        // Description: সর্বোচ্চ ৫০০ অক্ষর এবং এটি বাধ্যতামূলক (NOT NULL)
        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired();

        // Price: আর্থিক মূল্যের সঠিকতার জন্য decimal(18, 2) নির্ধারণ (১৮ ডিজিট, দশমিকের পর ২ ঘর)
        builder.Property(p => p.Price)
            .HasColumnName("price")
            .HasPrecision(18, 2)
            .IsRequired();

        // Category: সর্বোচ্চ ১০০ অক্ষর এবং বাধ্যতামূলক
        builder.Property(p => p.Category)
            .HasColumnName("category")
            .HasMaxLength(100)
            .IsRequired();

        // CreatedAt: তৈরির সময় এবং এটি বাধ্যতামূলক
        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
    }
}
