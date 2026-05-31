using MediatR;

namespace PayDa.Application.Matches.Queries.GetAdminPendingMatches;

public record GetAdminPendingMatchesQuery : IRequest<List<AdminPendingMatchDto>>;

public record AdminPendingMatchDto(
    Guid MatchId,
    decimal Amount,
    string Currency,
    decimal Price,
    DateTime PriceSetAt,
    DateTime ConfirmationDeadline,
    long RemainingSeconds,
    AdminPendingMatchPartyDto Sender,
    AdminPendingMatchPartyDto Receiver
);

public record AdminPendingMatchPartyDto(
    Guid UserId,
    string Name,
    bool IsTrusted,
    bool Confirmed
);
