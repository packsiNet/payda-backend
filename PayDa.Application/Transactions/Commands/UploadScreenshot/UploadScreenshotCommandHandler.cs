using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Transactions.Commands.UploadScreenshot;

public class UploadScreenshotCommandHandler : IRequestHandler<UploadScreenshotCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IStorageService _storage;

    public UploadScreenshotCommandHandler(IAppDbContext context, ICurrentUserService currentUser, IStorageService storage)
    {
        _context = context;
        _currentUser = currentUser;
        _storage = storage;
    }

    public async Task Handle(UploadScreenshotCommand cmd, CancellationToken ct)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Match)
                .ThenInclude(m => m.SenderRequest)
            .FirstOrDefaultAsync(t => t.Id == cmd.TransactionId, ct)
            ?? throw new NotFoundException("Transaction not found");

        if (transaction.Match.SenderRequest.UserId != _currentUser.UserId)
            throw new ForbiddenException("Only the sender can upload the screenshot");

        var url = await _storage.UploadAsync(cmd.File, cmd.FileName, "transactions/screenshots", ct);
        transaction.UploadScreenshot(url);
        await _context.SaveChangesAsync(ct);
    }
}
