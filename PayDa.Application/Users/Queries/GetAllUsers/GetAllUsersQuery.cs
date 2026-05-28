using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Users.Queries.GetAllUsers;

public record GetAllUsersQuery(int Page = 1, int PageSize = 20) : IRequest<List<UserSummaryDto>>;

public record UserSummaryDto(
    Guid Id,
    long TelegramId,
    string? TelegramUsername,
    string? FirstName,
    string? LastName,
    KycStatus KycStatus,
    UserRole Role,
    bool IsTrusted,
    string TierName,
    DateTime CreatedAt,
    string? SelfieImageUrl,
    string? DocumentImageUrl
);
