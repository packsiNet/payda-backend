using MediatR;

namespace PayDa.Application.Users.Queries.GetPendingKycUsers;

public record GetPendingKycUsersQuery(int Page = 1, int PageSize = 20) : IRequest<List<PendingKycUserDto>>;

public record PendingKycUserDto(
    Guid Id,
    long TelegramId,
    string? TelegramUsername,
    string? FirstName,
    string? LastName,
    string? DateOfBirth,
    string? PhoneNumber,
    string? SelfieImageUrl,
    string? DocumentImageUrl,
    DateTime? KycSubmittedAt
);
