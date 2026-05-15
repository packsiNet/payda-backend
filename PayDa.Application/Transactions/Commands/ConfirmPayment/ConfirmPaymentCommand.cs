using MediatR;

namespace PayDa.Application.Transactions.Commands.ConfirmPayment;

public record ConfirmPaymentCommand(Guid TransactionId) : IRequest;
