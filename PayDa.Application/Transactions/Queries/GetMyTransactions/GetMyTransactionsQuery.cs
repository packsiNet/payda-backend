using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Transactions.Queries.GetMyTransactions;

public record GetMyTransactionsQuery(
    RequestType? Type = null,
    TransactionStatus? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<List<MyTransactionDto>>;

public record MyTransactionDto(
    Guid Id,
    Guid MatchId,
    TransactionStatus Status,
    string? ReferenceCode,
    RequestType MyRole,
    Currency Currency,
    decimal Amount,
    decimal MatchPrice,
    string? PaymentMethod,
    string CounterpartDisplayName,
    int CounterpartLevel,
    string CounterpartLevelTitle,
    bool CounterpartIsTrusted,
    string? ScreenshotUrl,
    DateTime? PaidAt,
    DateTime? SettledAt,
    DateTime CreatedAt
);
