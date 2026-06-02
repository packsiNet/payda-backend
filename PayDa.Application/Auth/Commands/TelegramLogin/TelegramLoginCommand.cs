using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Auth.Commands.TelegramLogin;

public record TelegramLoginCommand(string InitData) : IRequest<TelegramLoginResult>;

public record TelegramLoginResult(
    string Token,
    bool IsNewUser,
    Guid UserId,
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
    string? DocumentImageUrl
);
