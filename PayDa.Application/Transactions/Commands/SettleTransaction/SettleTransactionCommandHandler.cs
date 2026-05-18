using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Transactions.Commands.SettleTransaction;

public class SettleTransactionCommandHandler : IRequestHandler<SettleTransactionCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SettleTransactionCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(SettleTransactionCommand cmd, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var transaction = await _context.Transactions
            .Include(t => t.Match)
                .ThenInclude(m => m.SenderRequest)
                    .ThenInclude(r => r.User)
            .Include(t => t.Match)
                .ThenInclude(m => m.ReceiverRequest)
                    .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(t => t.Id == cmd.TransactionId, ct)
            ?? throw new NotFoundException("Transaction not found");

        var isSender = transaction.Match.SenderRequest.UserId == userId;
        var isReceiver = transaction.Match.ReceiverRequest.UserId == userId;
        if (!isSender && !isReceiver)
            throw new ForbiddenException("You are not a participant in this transaction");

        if (transaction.Status != TransactionStatus.Confirmed)
            throw new ForbiddenException("Transaction must be confirmed before settlement");

        transaction.Settle();
        transaction.Match.Complete();

        transaction.Match.SenderRequest.User.IncrementCompletedTransactions();
        transaction.Match.ReceiverRequest.User.IncrementCompletedTransactions();

        await _context.SaveChangesAsync(ct);
    }
}
