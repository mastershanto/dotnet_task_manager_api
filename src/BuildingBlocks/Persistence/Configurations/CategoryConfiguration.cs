using Categories.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<CategoryModel>
{
    public void Configure(EntityTypeBuilder<CategoryModel> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(c => c.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(c => c.Color).HasColumnName("color").HasMaxLength(50);
        builder.Property(c => c.Icon).HasColumnName("icon").HasMaxLength(50);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").IsRequired();
    }
}
