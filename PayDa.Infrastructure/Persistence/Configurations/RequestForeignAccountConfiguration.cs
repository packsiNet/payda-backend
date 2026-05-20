using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayDa.Domain.Entities;

namespace PayDa.Infrastructure.Persistence.Configurations;

public class RequestForeignAccountConfiguration : IEntityTypeConfiguration<RequestForeignAccount>
{
    public void Configure(EntityTypeBuilder<RequestForeignAccount> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.FullName).IsRequired().HasMaxLength(200);
        builder.Property(f => f.Username).HasMaxLength(100);
        builder.Property(f => f.Email).HasMaxLength(200);
        builder.Property(f => f.EmailOrPhone).HasMaxLength(100);
        builder.Property(f => f.Iban).HasMaxLength(50);
        builder.Property(f => f.Bic).HasMaxLength(20);
        builder.Property(f => f.BankName).HasMaxLength(200);
        builder.Property(f => f.AccountNum).HasMaxLength(50);
        builder.Property(f => f.Swift).HasMaxLength(20);
        builder.Property(f => f.BankAddress).HasMaxLength(500);

        builder.HasOne(f => f.Request)
            .WithMany(r => r.ForeignAccounts)
            .HasForeignKey(f => f.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
