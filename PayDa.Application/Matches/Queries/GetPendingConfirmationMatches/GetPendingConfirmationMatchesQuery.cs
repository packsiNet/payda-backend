using MediatR;

namespace PayDa.Application.Matches.Queries.GetPendingConfirmationMatches;

public record GetPendingConfirmationMatchesQuery : IRequest<List<PendingConfirmationMatchDto>>;

public record PendingConfirmationMatchDto(
    Guid MatchId,
    Guid MyRequestId,
    decimal Amount,
    string Currency,
    decimal Price,
    DateTime PriceSetAt,
    DateTime ConfirmationDeadline,
    string CounterpartName,
    bool CounterpartIsTrusted,
    bool MyConfirmation
);
