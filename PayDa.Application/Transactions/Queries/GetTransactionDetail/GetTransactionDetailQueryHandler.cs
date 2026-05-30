using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Transactions.Queries.GetTransactionDetail;

public class GetTransactionDetailQueryHandler : IRequestHandler<GetTransactionDetailQuery, TransactionDetailDto>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetTransactionDetailQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<TransactionDetailDto> Handle(GetTransactionDetailQuery request, CancellationToken ct)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Match)
                .ThenInclude(m => m.SenderRequest)
            .Include(t => t.Match)
                .ThenInclude(m => m.ReceiverRequest)
                    .ThenInclude(r => r.ForeignAccounts)
            .FirstOrDefaultAsync(t => t.Id == request.TransactionId, ct)
            ?? throw new NotFoundException("Transaction not found");

        var userId = _currentUser.UserId;
        var isSender = transaction.Match.SenderRequest.UserId == userId;
        var isReceiver = transaction.Match.ReceiverRequest.UserId == userId;

        if (!isSender && !isReceiver)
            throw new ForbiddenException("Access denied");

        List<ForeignAccountDto>? receiverAccounts = null;
        if (isSender && transaction.Status >= TransactionStatus.TomanConfirmed)
        {
            receiverAccounts = transaction.Match.ReceiverRequest.ForeignAccounts
                .Select(a => new ForeignAccountDto(
                    a.Method, a.FullName, a.Username, a.Email,
                    a.EmailOrPhone, a.Iban, a.Bic, a.BankName,
                    a.AccountNum, a.Swift, a.BankAddress))
                .ToList();
        }

        return new TransactionDetailDto(
            transaction.Id,
            transaction.MatchId,
            transaction.Status,
            transaction.ReferenceCode,
            transaction.ForeignReceiptUrl,
            transaction.TomanDeclaredAt,
            transaction.TomanConfirmedAt,
            transaction.ForeignTransferredAt,
            transaction.CompletedAt,
            transaction.CreatedAt,
            receiverAccounts);
    }
}
