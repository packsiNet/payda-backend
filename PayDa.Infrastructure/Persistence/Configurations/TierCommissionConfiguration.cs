using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayDa.Domain.Entities;

namespace PayDa.Infrastructure.Persistence.Configurations;

public class TierCommissionConfiguration : IEntityTypeConfiguration<TierCommission>
{
    public void Configure(EntityTypeBuilder<TierCommission> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.TierId).IsUnique();

        builder.Property(c => c.SenderCommissionPercent).HasColumnType("decimal(5,4)").IsRequired();
        builder.Property(c => c.ReceiverCommissionPercent).HasColumnType("decimal(5,4)").IsRequired();

        builder.HasOne(c => c.Tier)
            .WithMany()
            .HasForeignKey(c => c.TierId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
