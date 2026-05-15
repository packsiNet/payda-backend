using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Transactions.Commands.ConfirmPayment;

public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ConfirmPaymentCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(ConfirmPaymentCommand cmd, CancellationToken ct)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Match)
                .ThenInclude(m => m.ReceiverRequest)
            .FirstOrDefaultAsync(t => t.Id == cmd.TransactionId, ct)
            ?? throw new NotFoundException("Transaction not found");

        if (transaction.Status != TransactionStatus.ScreenshotUploaded)
            throw new ForbiddenException("Transaction is not in ScreenshotUploaded state");

        if (transaction.Match.ReceiverRequest.UserId != _currentUser.UserId)
            throw new ForbiddenException("Only the receiver can confirm the payment");

        transaction.Confirm();
        await _context.SaveChangesAsync(ct);
    }
}
