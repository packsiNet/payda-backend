using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayDa.Domain.Entities;

namespace PayDa.Infrastructure.Persistence.Configurations;

public class TierConfiguration : IEntityTypeConfiguration<Tier>
{
    public void Configure(EntityTypeBuilder<Tier> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(50).IsRequired();
        builder.Property(t => t.MaxAmountPerRequest).HasColumnType("decimal(18,2)");
    }
}
