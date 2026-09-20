using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Products.Domain;

namespace Products.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<ProductModel>
{
    public void Configure(EntityTypeBuilder<ProductModel> builder)
    {
        builder.ToTable("products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.Title).HasColumnName("title").HasMaxLength(150).IsRequired();
        builder.Property(p => p.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
        builder.Property(p => p.Price).HasColumnName("price").HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.Category).HasColumnName("category").HasMaxLength(100).IsRequired();
        builder.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
    }
}
