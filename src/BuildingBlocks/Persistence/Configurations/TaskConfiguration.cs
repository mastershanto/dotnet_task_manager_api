using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tasks.Domain;

namespace BuildingBlocks.Persistence.Configurations;

/// <summary>
/// ডাটাবেস স্কিমা কনফিগারেশন (Fluent API Configuration):
/// Entity Framework Core-এ TaskItemModel-এর প্রপার্টিগুলো ডাটাবেস টেবিলের কোন কলামে কেমন টাইপে বসবে,
/// তা ডোমেন ক্লাসে না লিখে এই কনফিগারেশন ক্লাসে নির্ধারণ করা হলো (Clean Architecture)।
/// </summary>
public class TaskConfiguration : IEntityTypeConfiguration<TaskItemModel>
{
    public void Configure(EntityTypeBuilder<TaskItemModel> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id");

        builder.Property(t => t.Title)
            .HasColumnName("title")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.Priority)
            .HasColumnName("priority")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.DueDate)
            .HasColumnName("due_date");

        builder.Property(t => t.CategoryId)
            .HasColumnName("category_id");

        builder.Property(t => t.AssignedUserId)
            .HasColumnName("assigned_user_id");

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at");

        // Indexes for high performance queries
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.CategoryId);
        builder.HasIndex(t => t.CreatedAt);
    }
}
