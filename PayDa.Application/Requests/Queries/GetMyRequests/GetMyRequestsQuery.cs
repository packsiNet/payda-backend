using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Queries.GetMyRequests;

public record GetMyRequestsQuery(RequestType? Type = null) : IRequest<List<RequestSummaryDto>>;

public record RequestSummaryDto(
    Guid Id,
    RequestType Type,
    Currency Currency,
    decimal Amount,
    decimal RateValue,
    List<string> PaymentMethods,
    RequestStatus Status,
    DateTime ExpiresAt,
    DateTime CreatedAt,
    string? OwnerAvatarInitials,
    string? OwnerProfilePhotoUrl,
    string? OwnerDisplayName,
    bool OwnerIsTrusted,
    string OwnerTierName,
    int OwnerTierOrder
);
