using MediatR;

namespace PayDa.Application.Transactions.Commands.DisputeTransaction;

public record DisputeTransactionCommand(Guid TransactionId) : IRequest;
