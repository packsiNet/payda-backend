using MediatR;

namespace PayDa.Application.Transactions.Commands.UploadScreenshot;

public record UploadScreenshotCommand(Guid TransactionId, Stream File, string FileName) : IRequest;
