using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Transactions.Queries.GetAdminPendingTomanTransactions;

public class GetAdminPendingTomanTransactionsQueryHandler
    : IRequestHandler<GetAdminPendingTomanTransactionsQuery, List<AdminPendingTomanTransactionDto>>
{
    private readonly IAppDbContext _context;

    public GetAdminPendingTomanTransactionsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminPendingTomanTransactionDto>> Handle(
        GetAdminPendingTomanTransactionsQuery request, CancellationToken ct)
    {
        var transactions = await _context.Transactions
            .Include(t => t.Match)
                .ThenInclude(m => m.SenderRequest)
                    .ThenInclude(r => r.User)
            .Include(t => t.Match)
                .ThenInclude(m => m.ReceiverRequest)
                    .ThenInclude(r => r.User)
            .Where(t => t.Status == TransactionStatus.WaitingForTomanPayment)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(ct);

        return transactions.Select(t =>
        {
            var match = t.Match;
            var sender = match.SenderRequest.User;
            var receiver = match.ReceiverRequest.User;

            return new AdminPendingTomanTransactionDto(
                t.Id,
                match.Id,
                t.ReferenceCode!,
                match.SenderRequest.Amount,
                match.SenderRequest.Currency.ToString(),
                match.Price,
                match.CreatedAt,
                new AdminTomanPartyDto(
                    receiver.Id,
                    GetName(receiver),
                    receiver.IsTrusted
                ),
                new AdminTomanPartyDto(
                    sender.Id,
                    GetName(sender),
                    sender.IsTrusted
                )
            );
        }).ToList();
    }

    private static string GetName(Domain.Entities.User user)
    {
        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        return !string.IsNullOrWhiteSpace(fullName) ? fullName
            : user.TelegramUsername ?? user.TelegramId.ToString();
    }
}
