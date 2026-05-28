using PayDa.Domain.Common;
using PayDa.Domain.Enums;

namespace PayDa.Domain.Entities;

public class Match : BaseEntity
{
    public Guid SenderRequestId { get; private set; }
    public Request SenderRequest { get; private set; } = default!;

    public Guid ReceiverRequestId { get; private set; }
    public Request ReceiverRequest { get; private set; } = default!;

    public decimal Price { get; private set; }

    public bool IsAgentInvolved { get; private set; }
    public MatchStatus Status { get; private set; } = MatchStatus.Active;

    public DateTime? PriceSetAt { get; private set; }
    public DateTime? ConfirmationDeadline { get; private set; }
    public bool SenderConfirmed { get; private set; }
    public bool ReceiverConfirmed { get; private set; }

    public Transaction? Transaction { get; private set; }

    private Match() { }

    public static Match Create(Guid senderRequestId, Guid receiverRequestId,
        decimal price = 0, bool isAgentInvolved = false) => new()
    {
        SenderRequestId = senderRequestId,
        ReceiverRequestId = receiverRequestId,
        Price = price,
        IsAgentInvolved = isAgentInvolved,
        Status = MatchStatus.Active
    };

    public static Match CreatePending(Guid senderRequestId, Guid receiverRequestId,
        decimal price, bool isAgentInvolved, DateTime confirmationDeadline) => new()
    {
        SenderRequestId = senderRequestId,
        ReceiverRequestId = receiverRequestId,
        Price = price,
        IsAgentInvolved = isAgentInvolved,
        Status = MatchStatus.PendingConfirmation,
        PriceSetAt = DateTime.UtcNow,
        ConfirmationDeadline = confirmationDeadline,
        SenderConfirmed = false,
        ReceiverConfirmed = false
    };

    public bool ConfirmBySender()
    {
        SenderConfirmed = true;
        UpdatedAt = DateTime.UtcNow;
        if (!ReceiverConfirmed) return false;
        Status = MatchStatus.Active;
        return true;
    }

    public bool ConfirmByReceiver()
    {
        ReceiverConfirmed = true;
        UpdatedAt = DateTime.UtcNow;
        if (!SenderConfirmed) return false;
        Status = MatchStatus.Active;
        return true;
    }

    public void Reject()
    {
        Status = MatchStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete() { Status = MatchStatus.Completed; UpdatedAt = DateTime.UtcNow; }
    public void Cancel() { Status = MatchStatus.Cancelled; UpdatedAt = DateTime.UtcNow; }
}
