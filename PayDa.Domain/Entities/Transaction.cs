using PayDa.Domain.Common;
using PayDa.Domain.Enums;

namespace PayDa.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid MatchId { get; private set; }
    public Match Match { get; private set; } = default!;

    public string? ReferenceCode { get; private set; }

    public string? ForeignReceiptUrl { get; private set; }

    public TransactionStatus Status { get; private set; } = TransactionStatus.WaitingForTomanPayment;

    public DateTime? TomanDeclaredAt { get; private set; }
    public DateTime? TomanConfirmedAt { get; private set; }
    public DateTime? ForeignTransferredAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private Transaction() { }

    public static Transaction Create(Guid matchId) => new()
    {
        MatchId = matchId,
        Status = TransactionStatus.WaitingForTomanPayment,
        ReferenceCode = GenerateReferenceCode()
    };

    private static string GenerateReferenceCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var part1 = new string(Enumerable.Range(0, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        var part2 = new string(Enumerable.Range(0, 3).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        return $"TX-{part1}-{part2}";
    }

    public void DeclareTomanPayment()
    {
        Status = TransactionStatus.TomanPaymentDeclared;
        TomanDeclaredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmTomanPayment()
    {
        Status = TransactionStatus.TomanConfirmed;
        TomanConfirmedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UploadForeignReceipt(string url)
    {
        ForeignReceiptUrl = url;
        Status = TransactionStatus.ForeignReceiptUploaded;
        ForeignTransferredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = TransactionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Dispute()
    {
        Status = TransactionStatus.Disputed;
        UpdatedAt = DateTime.UtcNow;
    }
}
