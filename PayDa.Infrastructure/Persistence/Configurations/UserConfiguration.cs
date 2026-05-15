using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayDa.Domain.Entities;

namespace PayDa.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.TelegramId).IsUnique();
        builder.Property(u => u.TelegramUsername).HasMaxLength(100);
        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);

        builder.HasOne(u => u.Tier)
            .WithMany()
            .HasForeignKey(u => u.TierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
