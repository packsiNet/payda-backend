using PayDa.Domain.Common;
using PayDa.Domain.Enums;

namespace PayDa.Domain.Entities;

public class User : BaseEntity
{
    public long TelegramId { get; private set; }
    public string? TelegramUsername { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? ProfilePhotoUrl { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? DateOfBirth { get; private set; }
    public string? Country { get; private set; }

    public KycStatus KycStatus { get; private set; } = KycStatus.Pending;
    public string? SelfieImageUrl { get; private set; }
    public string? DocumentImageUrl { get; private set; }
    public DateTime? KycSubmittedAt { get; private set; }
    public DateTime? KycReviewedAt { get; private set; }

    public UserRole Role { get; private set; } = UserRole.User;
    public bool IsTrusted { get; private set; } = false;

    public Guid TierId { get; private set; }
    public Tier Tier { get; private set; } = default!;
    public int CompletedTransactionsCount { get; private set; } = 0;

    private readonly List<Request> _requests = new();
    public IReadOnlyCollection<Request> Requests => _requests.AsReadOnly();

    private readonly List<Receiver> _receivers = new();
    public IReadOnlyCollection<Receiver> Receivers => _receivers.AsReadOnly();

    private User() { }

    public static User Create(long telegramId, string? username, string? firstName, string? lastName, string? profilePhotoUrl) => new()
    {
        TelegramId = telegramId,
        TelegramUsername = username,
        FirstName = firstName,
        LastName = lastName,
        ProfilePhotoUrl = profilePhotoUrl
    };

    public void UpdateTelegramProfile(string? username, string? firstName, string? lastName, string? profilePhotoUrl)
    {
        TelegramUsername = username;
        FirstName = firstName;
        LastName = lastName;
        ProfilePhotoUrl = profilePhotoUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SubmitKyc(string firstName, string lastName, string phone,
        string dateOfBirth, string selfieUrl, string documentUrl)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phone;
        DateOfBirth = dateOfBirth;
        SelfieImageUrl = selfieUrl;
        DocumentImageUrl = documentUrl;
        KycStatus = KycStatus.Pending;
        KycSubmittedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApproveKyc()
    {
        KycStatus = KycStatus.Approved;
        KycReviewedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RejectKyc()
    {
        KycStatus = KycStatus.Rejected;
        KycReviewedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetTrusted(bool trusted) { IsTrusted = trusted; UpdatedAt = DateTime.UtcNow; }
    public void SetRole(UserRole role) { Role = role; UpdatedAt = DateTime.UtcNow; }

    public void IncrementCompletedTransactions()
    {
        CompletedTransactionsCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpgradeTier(Guid newTierId)
    {
        TierId = newTierId;
        UpdatedAt = DateTime.UtcNow;
    }
}
