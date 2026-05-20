using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayDa.Domain.Entities;
using PayDa.Domain.Enums;

namespace PayDa.Infrastructure.Persistence.Configurations;

public class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Amount).HasColumnType("decimal(18,2)");
        builder.Property(r => r.RateValue).HasColumnType("decimal(18,2)");
        builder.Property(r => r.CommissionPercent).HasColumnType("decimal(5,2)");
        builder.Property(r => r.CommissionAmount).HasColumnType("decimal(18,2)");

        builder.Property(r => r.PaymentMethods)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<PaymentMethod>>(v, (JsonSerializerOptions?)null)!
            );

        builder.HasOne(r => r.User)
            .WithMany(u => u.Requests)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Receiver)
            .WithMany()
            .HasForeignKey(r => r.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Match)
            .WithMany()
            .HasForeignKey(r => r.MatchId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
