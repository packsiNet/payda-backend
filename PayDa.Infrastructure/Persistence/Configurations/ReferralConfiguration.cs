using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayDa.Domain.Entities;

namespace PayDa.Infrastructure.Persistence.Configurations;

public class ReferralConfiguration : IEntityTypeConfiguration<Referral>
{
    public void Configure(EntityTypeBuilder<Referral> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => r.ReferredId).IsUnique();

        builder.HasOne(r => r.Referrer)
            .WithMany()
            .HasForeignKey(r => r.ReferrerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Referred)
            .WithMany()
            .HasForeignKey(r => r.ReferredId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
