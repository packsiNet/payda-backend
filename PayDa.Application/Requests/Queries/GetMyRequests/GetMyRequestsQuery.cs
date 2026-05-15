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
    RequestStatus Status,
    DateTime ExpiresAt,
    DateTime CreatedAt
);
