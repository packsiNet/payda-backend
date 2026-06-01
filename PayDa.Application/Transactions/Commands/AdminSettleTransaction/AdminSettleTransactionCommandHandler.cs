using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Transactions.Commands.AdminSettleTransaction;

public class AdminSettleTransactionCommandHandler : IRequestHandler<AdminSettleTransactionCommand>
{
    private readonly IAppDbContext _context;

    public AdminSettleTransactionCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AdminSettleTransactionCommand cmd, CancellationToken ct)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Match)
                .ThenInclude(m => m.SenderRequest)
                    .ThenInclude(r => r.User)
            .Include(t => t.Match)
                .ThenInclude(m => m.ReceiverRequest)
                    .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(t => t.Id == cmd.TransactionId, ct)
            ?? throw new NotFoundException("Transaction not found");

        if (transaction.Status != TransactionStatus.ForeignReceiptConfirmed)
            throw new BadRequestException("Transaction is not in ForeignReceiptConfirmed state");

        transaction.Complete();
        transaction.Match.Complete();

        transaction.Match.SenderRequest.User.IncrementCompletedTransactions();
        transaction.Match.ReceiverRequest.User.IncrementCompletedTransactions();

        await _context.SaveChangesAsync(ct);
    }
}
