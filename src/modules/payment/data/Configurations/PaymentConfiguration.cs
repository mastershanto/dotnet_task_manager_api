using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Domain;

namespace Payments.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<PaymentModel>
{
    public void Configure(EntityTypeBuilder<PaymentModel> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(p => p.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.Currency).HasColumnName("currency").HasMaxLength(10).IsRequired();
        builder.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.HasIndex(p => p.UserId);
    }
}
