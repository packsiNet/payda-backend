using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Queries.GetAdminRequests;

public record GetAdminRequestsQuery(
    RequestType Type,
    Currency Currency,
    decimal? Amount = null
) : IRequest<List<AdminRequestDto>>;

public record AdminRequestDto(
    Guid Id,
    RequestType Type,
    Currency Currency,
    decimal Amount,
    decimal RateValue,
    List<string> PaymentMethods,
    DateTime ExpiresAt,
    DateTime CreatedAt,
    string UserDisplayName,
    string UserInitials,
    int UserTierOrder,
    string UserTierName,
    bool UserIsTrusted
);
