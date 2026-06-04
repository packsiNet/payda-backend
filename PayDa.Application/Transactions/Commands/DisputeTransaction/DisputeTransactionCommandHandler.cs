using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Transactions.Commands.DisputeTransaction;

public class DisputeTransactionCommandHandler : IRequestHandler<DisputeTransactionCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DisputeTransactionCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DisputeTransactionCommand cmd, CancellationToken ct)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Match)
                .ThenInclude(m => m.SenderRequest)
            .Include(t => t.Match)
                .ThenInclude(m => m.ReceiverRequest)
            .FirstOrDefaultAsync(t => t.Id == cmd.TransactionId, ct)
            ?? throw new NotFoundException("Transaction not found");

        var userId = _currentUser.UserId;
        var isSender = transaction.Match.SenderRequest.UserId == userId;
        var isReceiver = transaction.Match.ReceiverRequest.UserId == userId;

        if (!isSender && !isReceiver)
            throw new ForbiddenException("You are not a participant in this transaction");

        if (transaction.Status == TransactionStatus.Completed ||
            transaction.Status == TransactionStatus.Disputed)
            throw new BadRequestException("Transaction cannot be disputed in its current state");

        transaction.Dispute();
        await _context.SaveChangesAsync(ct);
    }
}
