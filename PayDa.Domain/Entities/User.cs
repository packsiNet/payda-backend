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

    public KycStatus KycStatus { get; private set; } = KycStatus.NotSubmitted;
    public string? SelfieImageUrl { get; private set; }
    public string? DocumentImageUrl { get; private set; }
    public DateTime? KycSubmittedAt { get; private set; }
    public DateTime? KycReviewedAt { get; private set; }

    public UserRole Role { get; private set; } = UserRole.User;
    public bool IsTrusted { get; private set; } = false;

    public Guid TierId { get; private set; }
    public Tier Tier { get; private set; } = default!;
    public int CompletedTransactionsCount { get; private set; } = 0;

    public string ReferralCode { get; private set; } = default!;
    public Guid? ReferredById { get; private set; }

    private readonly List<Request> _requests = new();
    public IReadOnlyCollection<Request> Requests => _requests.AsReadOnly();

    private readonly List<Receiver> _receivers = new();
    public IReadOnlyCollection<Receiver> Receivers => _receivers.AsReadOnly();

    private User() { }

    public static User Create(long telegramId, string? username, string? firstName, string? lastName, string? profilePhotoUrl, string referralCode) => new()
    {
        TelegramId = telegramId,
        TelegramUsername = username,
        FirstName = firstName,
        LastName = lastName,
        ProfilePhotoUrl = profilePhotoUrl,
        ReferralCode = referralCode
    };

    public void ApplyReferral(Guid referredById)
    {
        if (ReferredById is not null)
            throw new InvalidOperationException("Referral already applied.");
        if (Id == referredById)
            throw new InvalidOperationException("Cannot use own referral code.");
        ReferredById = referredById;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTelegramProfile(string? username, string? firstName, string? lastName, string? profilePhotoUrl)
    {
        TelegramUsername = username;
        FirstName = firstName;
        LastName = lastName;
        ProfilePhotoUrl = profilePhotoUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPhoneNumber(string phoneNumber)
    {
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SubmitKyc(string firstName, string lastName,
        string dateOfBirth, string selfieUrl, string documentUrl)
    {
        if (string.IsNullOrEmpty(PhoneNumber))
            throw new InvalidOperationException("Phone number must be verified before submitting KYC.");

        FirstName = firstName;
        LastName = lastName;
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
