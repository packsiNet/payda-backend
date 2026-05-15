using MediatR;

namespace PayDa.Application.Transactions.Commands.SettleTransaction;

public record SettleTransactionCommand(Guid TransactionId) : IRequest;
