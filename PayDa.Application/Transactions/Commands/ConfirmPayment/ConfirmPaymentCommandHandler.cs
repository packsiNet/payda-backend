using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Transactions.Commands.ConfirmPayment;

public class ConfirmTomanPaymentCommandHandler : IRequestHandler<ConfirmTomanPaymentCommand>
{
    private readonly IAppDbContext _context;

    public ConfirmTomanPaymentCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ConfirmTomanPaymentCommand cmd, CancellationToken ct)
    {
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == cmd.TransactionId, ct)
            ?? throw new NotFoundException("Transaction not found");

        if (transaction.Status != TransactionStatus.WaitingForTomanPayment &&
            transaction.Status != TransactionStatus.TomanPaymentDeclared)
            throw new BadRequestException("Transaction is not in a confirmable toman payment state");

        transaction.ConfirmTomanPayment();
        await _context.SaveChangesAsync(ct);
    }
}
