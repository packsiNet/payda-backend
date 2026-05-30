using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Transactions.Commands.UploadScreenshot;

public class DeclareTomanPaymentCommandHandler : IRequestHandler<DeclareTomanPaymentCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeclareTomanPaymentCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeclareTomanPaymentCommand cmd, CancellationToken ct)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Match)
                .ThenInclude(m => m.ReceiverRequest)
            .FirstOrDefaultAsync(t => t.Id == cmd.TransactionId, ct)
            ?? throw new NotFoundException("Transaction not found");

        if (transaction.Status != TransactionStatus.WaitingForTomanPayment)
            throw new BadRequestException("Transaction is not waiting for toman payment");

        if (transaction.Match.ReceiverRequest.UserId != _currentUser.UserId)
            throw new ForbiddenException("Only the receiver can declare toman payment");

        transaction.DeclareTomanPayment();
        await _context.SaveChangesAsync(ct);
    }
}
