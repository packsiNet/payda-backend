using PayDa.Domain.Common;
using PayDa.Domain.Enums;

namespace PayDa.Domain.Entities;

public class Match : BaseEntity
{
    public Guid SenderRequestId { get; private set; }
    public Request SenderRequest { get; private set; } = default!;

    public Guid ReceiverRequestId { get; private set; }
    public Request ReceiverRequest { get; private set; } = default!;

    public bool IsAgentInvolved { get; private set; }
    public MatchStatus Status { get; private set; } = MatchStatus.Active;

    public Transaction? Transaction { get; private set; }

    private Match() { }

    public static Match Create(Guid senderRequestId, Guid receiverRequestId,
        bool isAgentInvolved = false) => new()
    {
        SenderRequestId = senderRequestId,
        ReceiverRequestId = receiverRequestId,
        IsAgentInvolved = isAgentInvolved,
        Status = MatchStatus.Active
    };

    public void Complete() { Status = MatchStatus.Completed; UpdatedAt = DateTime.UtcNow; }
    public void Cancel() { Status = MatchStatus.Cancelled; UpdatedAt = DateTime.UtcNow; }
}
