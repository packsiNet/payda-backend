using PayDa.Domain.Common;

namespace PayDa.Domain.Entities;

public class TierCommission : BaseEntity
{
    public Guid TierId { get; private set; }
    public Tier Tier { get; private set; } = default!;

    public decimal SenderCommissionPercent { get; private set; }
    public decimal ReceiverCommissionPercent { get; private set; }

    private TierCommission() { }

    public static TierCommission Create(Guid tierId, decimal senderPercent, decimal receiverPercent) => new()
    {
        TierId = tierId,
        SenderCommissionPercent = senderPercent,
        ReceiverCommissionPercent = receiverPercent
    };

    public void Update(decimal senderPercent, decimal receiverPercent)
    {
        SenderCommissionPercent = senderPercent;
        ReceiverCommissionPercent = receiverPercent;
        UpdatedAt = DateTime.UtcNow;
    }
}
