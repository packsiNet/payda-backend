using MediatR;

namespace PayDa.Application.Transactions.Commands.ConfirmPayment;

public record ConfirmTomanPaymentCommand(Guid TransactionId) : IRequest;
