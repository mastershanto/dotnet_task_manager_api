using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projects.Domain;

namespace BuildingBlocks.Persistence.Configurations;

/// <summary>
/// ProjectModel-এর ডাটাবেস স্কিমা কনফিগারেশন (Fluent API):
/// </summary>
public class ProjectConfiguration : IEntityTypeConfiguration<ProjectModel>
{
    public void Configure(EntityTypeBuilder<ProjectModel> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("id");

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.Color)
            .HasColumnName("color")
            .HasMaxLength(50);

        builder.Property(p => p.StartDate)
            .HasColumnName("start_date");

        builder.Property(p => p.EndDate)
            .HasColumnName("end_date");

        builder.Property(p => p.OwnerId)
            .HasColumnName("owner_id");

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at");

        // Indexes
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.CreatedAt);
    }
}
