using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Matches.Queries.GetMyMatches;

public class GetMyMatchesQueryHandler : IRequestHandler<GetMyMatchesQuery, List<MyMatchDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyMatchesQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<MyMatchDto>> Handle(GetMyMatchesQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var matches = await _context.Matches
            .Include(m => m.SenderRequest).ThenInclude(r => r.User).ThenInclude(u => u.Tier)
            .Include(m => m.ReceiverRequest).ThenInclude(r => r.User).ThenInclude(u => u.Tier)
            .Include(m => m.Transaction)
            .Where(m =>
                m.SenderRequest.UserId == userId ||
                m.ReceiverRequest.UserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(ct);

        return matches
            .Select(m =>
            {
                var isSender = m.SenderRequest.UserId == userId;
                var myRequest = isSender ? m.SenderRequest : m.ReceiverRequest;
                var counterpartRequest = isSender ? m.ReceiverRequest : m.SenderRequest;
                var counterpart = counterpartRequest.User;

                var hasFullName = !string.IsNullOrEmpty(counterpart.FirstName) && !string.IsNullOrEmpty(counterpart.LastName);
                var counterpartName = hasFullName
                    ? $"{counterpart.FirstName} {counterpart.LastName![0]}."
                    : counterpart.TelegramUsername ?? "Unknown";

                return new MyMatchDto(
                    m.Id,
                    myRequest.Id,
                    myRequest.Type,
                    myRequest.Currency,
                    myRequest.Amount,
                    myRequest.PricePreference,
                    m.Price,
                    myRequest.ExpiresAt,
                    myRequest.CreatedAt,
                    m.CreatedAt,
                    counterpartName,
                    counterpart.Tier?.Order ?? 0,
                    counterpart.Tier?.Name ?? string.Empty,
                    counterpart.IsTrusted,
                    counterpartRequest.PaymentMethods?.Select(p => p.ToString()).ToList() ?? [],
                    m.Transaction?.Id,
                    m.Transaction?.Status
                );
            })
            .ToList();
    }
}
