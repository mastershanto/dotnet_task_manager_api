using Auth.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Persistence.Configurations;

public class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCodeModel>
{
    public void Configure(EntityTypeBuilder<OtpCodeModel> builder)
    {
        builder.ToTable("otp_codes");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasColumnName("id");
        builder.Property(o => o.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.Property(o => o.Code).HasColumnName("code").HasMaxLength(10).IsRequired();
        builder.Property(o => o.Purpose).HasColumnName("purpose").IsRequired();
        builder.Property(o => o.ExpiresAt).HasColumnName("expires_at").IsRequired();
        builder.Property(o => o.IsUsed).HasColumnName("is_used").HasDefaultValue(false).IsRequired();
        builder.Property(o => o.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(o => new { o.Email, o.Purpose });
    }
}
