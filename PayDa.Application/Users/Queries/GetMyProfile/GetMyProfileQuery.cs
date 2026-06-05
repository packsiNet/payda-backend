using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Users.Queries.GetMyProfile;

public record GetMyProfileQuery : IRequest<UserProfileDto>;

public record UserProfileDto(
    Guid Id,
    long TelegramId,
    string? TelegramUsername,
    string? FirstName,
    string? LastName,
    KycStatus KycStatus,
    UserRole Role,
    bool IsTrusted,
    string TierName,
    int TierOrder,
    int CompletedTransactionsCount,
    bool PhoneVerified,
    string? SelfieImageUrl,
    string? DocumentImageUrl,
    string ReferralCode
);
