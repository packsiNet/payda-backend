using PayDa.Domain.Common;
using PayDa.Domain.Enums;

namespace PayDa.Domain.Entities;

public class Request : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public RequestType Type { get; private set; }
    public Currency Currency { get; private set; }
    public decimal Amount { get; private set; }

    public PricePreference PricePreference { get; private set; }

    public List<PaymentMethod> PaymentMethods { get; private set; } = new();

    public Guid? ReceiverId { get; private set; }
    public Receiver? Receiver { get; private set; }

    public List<RequestForeignAccount> ForeignAccounts { get; private set; } = new();

    public RequestStatus Status { get; private set; } = RequestStatus.Pending;
    public DateTime ExpiresAt { get; private set; }

    public Guid? MatchId { get; private set; }
    public Match? Match { get; private set; }

    private Request() { }

    public static Request Create(Guid userId, RequestType type, Currency currency,
        decimal amount, PricePreference pricePreference,
        List<PaymentMethod> paymentMethods, Guid? receiverId, DateTime expiresAt) => new()
    {
        UserId = userId,
        Type = type,
        Currency = currency,
        Amount = amount,
        PricePreference = pricePreference,
        PaymentMethods = paymentMethods,
        ReceiverId = receiverId,
        ExpiresAt = expiresAt,
        Status = RequestStatus.Pending
    };

    public void SetMatched(Guid matchId) { Status = RequestStatus.Matched; MatchId = matchId; UpdatedAt = DateTime.UtcNow; }
    public void Cancel() { Status = RequestStatus.Cancelled; UpdatedAt = DateTime.UtcNow; }
    public void Expire() { Status = RequestStatus.Expired; UpdatedAt = DateTime.UtcNow; }
}
