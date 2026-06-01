using MediatR;

namespace PayDa.Application.Transactions.Commands.AdminSettleTransaction;

public record AdminSettleTransactionCommand(Guid TransactionId) : IRequest;
