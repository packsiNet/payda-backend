using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Transactions.Commands.SettleTransaction;

public class UploadForeignReceiptCommandHandler : IRequestHandler<UploadForeignReceiptCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IStorageService _storage;

    public UploadForeignReceiptCommandHandler(IAppDbContext context, ICurrentUserService currentUser, IStorageService storage)
    {
        _context = context;
        _currentUser = currentUser;
        _storage = storage;
    }

    public async Task Handle(UploadForeignReceiptCommand cmd, CancellationToken ct)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Match)
                .ThenInclude(m => m.SenderRequest)
            .FirstOrDefaultAsync(t => t.Id == cmd.TransactionId, ct)
            ?? throw new NotFoundException("Transaction not found");

        if (transaction.Status != TransactionStatus.TomanConfirmed)
            throw new BadRequestException("Transaction is not in TomanConfirmed state");

        if (transaction.Match.SenderRequest.UserId != _currentUser.UserId)
            throw new ForbiddenException("Only the sender can upload the foreign receipt");

        var url = await _storage.UploadAsync(cmd.File, cmd.FileName, "transactions/foreign-receipts", ct);
        transaction.UploadForeignReceipt(url);
        await _context.SaveChangesAsync(ct);
    }
}
