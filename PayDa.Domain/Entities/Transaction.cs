using PayDa.Domain.Common;
using PayDa.Domain.Enums;

namespace PayDa.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid MatchId { get; private set; }
    public Match Match { get; private set; } = default!;

    public string? ReferenceCode { get; private set; }
    public string? ScreenshotUrl { get; private set; }
    public TransactionStatus Status { get; private set; } = TransactionStatus.Pending;

    public DateTime? PaidAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? SettledAt { get; private set; }

    private Transaction() { }

    public static Transaction Create(Guid matchId) => new()
    {
        MatchId = matchId,
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

    public void UploadScreenshot(string url)
    {
        ScreenshotUrl = url;
        Status = TransactionStatus.ScreenshotUploaded;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        Status = TransactionStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Settle()
    {
        Status = TransactionStatus.Settled;
        SettledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Dispute()
    {
        Status = TransactionStatus.Disputed;
        UpdatedAt = DateTime.UtcNow;
    }
}
