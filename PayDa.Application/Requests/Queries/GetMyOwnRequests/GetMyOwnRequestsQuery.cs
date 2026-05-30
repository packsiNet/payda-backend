using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Queries.GetMyOwnRequests;

public record GetMyOwnRequestsQuery(RequestType? Type = null) : IRequest<List<MyOwnRequestDto>>;

public record MyOwnRequestDto(
    Guid Id,
    RequestType Type,
    Currency Currency,
    decimal Amount,
    PricePreference PricePreference,
    List<string> PaymentMethods,
    RequestStatus Status,
    Guid? MatchId,
    DateTime ExpiresAt,
    DateTime CreatedAt
);
