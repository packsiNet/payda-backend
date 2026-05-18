using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayDa.Domain.Entities;

namespace PayDa.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.ReferenceCode).HasMaxLength(20);
        builder.HasIndex(t => t.ReferenceCode).IsUnique();
        builder.Property(t => t.ScreenshotUrl).HasMaxLength(500);
    }
}
