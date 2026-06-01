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

    // Receive type only — info about the person who pays Tomans
    public string? TomanPayerFullName { get; private set; }
    public string? TomanPayerMobileNumber { get; private set; }

    private Request() { }

    public static Request Create(Guid userId, RequestType type, Currency currency,
        decimal amount, PricePreference pricePreference,
        List<PaymentMethod> paymentMethods, Guid? receiverId, DateTime expiresAt,
        string? tomanPayerFullName = null,
        string? tomanPayerMobileNumber = null) => new()
    {
        UserId = userId,
        Type = type,
        Currency = currency,
        Amount = amount,
        PricePreference = pricePreference,
        PaymentMethods = paymentMethods,
        ReceiverId = receiverId,
        ExpiresAt = expiresAt,
        Status = RequestStatus.Pending,
        TomanPayerFullName = tomanPayerFullName,
        TomanPayerMobileNumber = tomanPayerMobileNumber
    };

    public void SetMatched(Guid matchId) { Status = RequestStatus.Matched; MatchId = matchId; UpdatedAt = DateTime.UtcNow; }
    public void ResetToPending() { Status = RequestStatus.Pending; MatchId = null; UpdatedAt = DateTime.UtcNow; }
    public void Cancel() { Status = RequestStatus.Cancelled; UpdatedAt = DateTime.UtcNow; }
    public void Expire() { Status = RequestStatus.Expired; UpdatedAt = DateTime.UtcNow; }
}
