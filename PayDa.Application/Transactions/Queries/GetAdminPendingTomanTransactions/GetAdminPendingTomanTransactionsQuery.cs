using MediatR;

namespace PayDa.Application.Transactions.Queries.GetAdminPendingTomanTransactions;

public record GetAdminPendingTomanTransactionsQuery : IRequest<List<AdminPendingTomanTransactionDto>>;

public record AdminPendingTomanTransactionDto(
    Guid TransactionId,
    Guid MatchId,
    string ReferenceCode,
    decimal Amount,
    string Currency,
    decimal Price,
    DateTime MatchDate,
    AdminTomanPartyDto TomanPayer,
    AdminTomanPartyDto ForeignSender
);

public record AdminTomanPartyDto(
    Guid UserId,
    string Name,
    bool IsTrusted
);
