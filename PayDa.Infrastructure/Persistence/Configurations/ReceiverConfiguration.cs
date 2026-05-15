using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayDa.Domain.Entities;

namespace PayDa.Infrastructure.Persistence.Configurations;

public class ReceiverConfiguration : IEntityTypeConfiguration<Receiver>
{
    public void Configure(EntityTypeBuilder<Receiver> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(r => r.LastName).HasMaxLength(100).IsRequired();
        builder.Property(r => r.NationalId).HasMaxLength(20).IsRequired();
        builder.Property(r => r.MobileNumber).HasMaxLength(20).IsRequired();
        builder.Property(r => r.IBAN).HasMaxLength(34).IsRequired();
    }
}
