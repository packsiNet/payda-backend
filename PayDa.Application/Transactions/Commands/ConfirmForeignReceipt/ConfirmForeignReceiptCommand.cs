using MediatR;

namespace PayDa.Application.Transactions.Commands.ConfirmForeignReceipt;

public record ConfirmForeignReceiptCommand(Guid TransactionId) : IRequest;
