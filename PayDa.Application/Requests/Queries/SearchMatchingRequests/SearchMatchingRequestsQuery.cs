using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Queries.SearchMatchingRequests;

public record SearchMatchingRequestsQuery(
    RequestType Type,
    Currency Currency,
    decimal Amount
) : IRequest<List<MatchingRequestDto>>;

public record MatchingRequestDto(
    Guid RequestId,
    string UserInitials,
    string UserDisplayName,
    int UserLevel,
    string UserLevelTitle,
    bool IsTrusted,
    decimal Amount,
    PricePreference PricePreference,
    List<string> PaymentMethods,
    DateTime CreatedAt
);
