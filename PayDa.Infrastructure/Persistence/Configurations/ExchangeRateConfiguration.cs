using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayDa.Domain.Entities;

namespace PayDa.Infrastructure.Persistence.Configurations;

public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
    public void Configure(EntityTypeBuilder<ExchangeRate> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Currency).IsUnique();
        builder.Property(e => e.MarketRate).HasColumnType("decimal(18,2)");
        builder.Property(e => e.InstantRate).HasColumnType("decimal(18,2)");
    }
}
