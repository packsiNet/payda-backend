using MediatR;

namespace PayDa.Application.Transactions.Commands.UploadScreenshot;

public record DeclareTomanPaymentCommand(Guid TransactionId) : IRequest;
