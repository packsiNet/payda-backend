using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Transactions.Queries.GetTransactionDetail;

public record GetTransactionDetailQuery(Guid TransactionId) : IRequest<TransactionDetailDto>;

public record ForeignAccountDto(
    PaymentMethod Method,
    string FullName,
    string? Username,
    string? Email,
    string? EmailOrPhone,
    string? Iban,
    string? Bic,
    string? BankName,
    string? AccountNum,
    string? Swift,
    string? BankAddress
);

public record TransactionDetailDto(
    Guid Id,
    Guid MatchId,
    TransactionStatus Status,
    string? ReferenceCode,
    string? ForeignReceiptUrl,
    DateTime? TomanDeclaredAt,
    DateTime? TomanConfirmedAt,
    DateTime? ForeignTransferredAt,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    List<ForeignAccountDto>? ReceiverForeignAccounts
);
