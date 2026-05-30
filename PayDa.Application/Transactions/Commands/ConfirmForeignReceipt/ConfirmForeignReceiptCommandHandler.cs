using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Transactions.Commands.ConfirmForeignReceipt;

public class ConfirmForeignReceiptCommandHandler : IRequestHandler<ConfirmForeignReceiptCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ConfirmForeignReceiptCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(ConfirmForeignReceiptCommand cmd, CancellationToken ct)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Match)
                .ThenInclude(m => m.ReceiverRequest)
                    .ThenInclude(r => r.User)
            .Include(t => t.Match)
                .ThenInclude(m => m.SenderRequest)
                    .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(t => t.Id == cmd.TransactionId, ct)
            ?? throw new NotFoundException("Transaction not found");

        if (transaction.Status != TransactionStatus.ForeignReceiptUploaded)
            throw new BadRequestException("Transaction is not in ForeignReceiptUploaded state");

        if (transaction.Match.ReceiverRequest.UserId != _currentUser.UserId)
            throw new ForbiddenException("Only the receiver can confirm the foreign receipt");

        transaction.Complete();
        transaction.Match.Complete();

        transaction.Match.SenderRequest.User.IncrementCompletedTransactions();
        transaction.Match.ReceiverRequest.User.IncrementCompletedTransactions();

        await _context.SaveChangesAsync(ct);
    }
}
