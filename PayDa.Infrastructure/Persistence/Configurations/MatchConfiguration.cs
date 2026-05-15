using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayDa.Domain.Entities;

namespace PayDa.Infrastructure.Persistence.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasOne(m => m.SenderRequest)
            .WithMany()
            .HasForeignKey(m => m.SenderRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.ReceiverRequest)
            .WithMany()
            .HasForeignKey(m => m.ReceiverRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Transaction)
            .WithOne(t => t.Match)
            .HasForeignKey<Transaction>(t => t.MatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
