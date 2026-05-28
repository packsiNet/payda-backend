using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Transactions.Queries.GetMyTransactions;

public class GetMyTransactionsQueryHandler : IRequestHandler<GetMyTransactionsQuery, List<MyTransactionDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyTransactionsQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<MyTransactionDto>> Handle(GetMyTransactionsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var query = _context.Transactions
            .Include(t => t.Match)
                .ThenInclude(m => m.SenderRequest)
                    .ThenInclude(r => r.User).ThenInclude(u => u.Tier)
            .Include(t => t.Match)
                .ThenInclude(m => m.ReceiverRequest)
                    .ThenInclude(r => r.User).ThenInclude(u => u.Tier)
            .Where(t =>
                t.Match.SenderRequest.UserId == userId ||
                t.Match.ReceiverRequest.UserId == userId);

        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);

        if (request.Type.HasValue)
            query = request.Type.Value == RequestType.Send
                ? query.Where(t => t.Match.SenderRequest.UserId == userId)
                : query.Where(t => t.Match.ReceiverRequest.UserId == userId);

        var transactions = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return transactions.Select(t =>
        {
            var isSender = t.Match.SenderRequest.UserId == userId;
            var myRequest = isSender ? t.Match.SenderRequest : t.Match.ReceiverRequest;
            var counterpartRequest = isSender ? t.Match.ReceiverRequest : t.Match.SenderRequest;
            var counterpart = counterpartRequest.User;

            var counterpartName = counterpart.FirstName != null && counterpart.LastName != null
                ? $"{counterpart.FirstName} {counterpart.LastName[0]}."
                : "Unknown";

            return new MyTransactionDto(
                t.Id,
                t.MatchId,
                t.Status,
                t.ReferenceCode,
                isSender ? RequestType.Send : RequestType.Receive,
                myRequest.Currency,
                myRequest.Amount,
                t.Match.Price,
                myRequest.PaymentMethods.FirstOrDefault().ToString(),
                counterpartName,
                counterpart.Tier.Order,
                counterpart.Tier.Name,
                counterpart.IsTrusted,
                t.ScreenshotUrl,
                t.PaidAt,
                t.SettledAt,
                t.CreatedAt
            );
        }).ToList();
    }
}
