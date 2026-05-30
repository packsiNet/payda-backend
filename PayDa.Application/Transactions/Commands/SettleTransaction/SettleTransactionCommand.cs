using MediatR;

namespace PayDa.Application.Transactions.Commands.SettleTransaction;

public record UploadForeignReceiptCommand(Guid TransactionId, Stream File, string FileName) : IRequest;
