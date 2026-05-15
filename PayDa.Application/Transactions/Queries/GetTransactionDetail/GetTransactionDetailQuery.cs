using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Transactions.Queries.GetTransactionDetail;

public record GetTransactionDetailQuery(Guid TransactionId) : IRequest<TransactionDetailDto>;

public record TransactionDetailDto(
    Guid Id,
    Guid MatchId,
    TransactionStatus Status,
    string? ScreenshotUrl,
    DateTime? PaidAt,
    DateTime? ConfirmedAt,
    DateTime? SettledAt,
    DateTime CreatedAt
);
