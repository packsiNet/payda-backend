using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Queries.SearchMatchingRequests;

public class SearchMatchingRequestsQueryHandler
    : IRequestHandler<SearchMatchingRequestsQuery, List<MatchingRequestDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SearchMatchingRequestsQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<MatchingRequestDto>> Handle(
        SearchMatchingRequestsQuery query, CancellationToken ct)
    {
        var oppositeType = query.Type == RequestType.Send ? RequestType.Receive : RequestType.Send;

        var rows = await _context.Requests
            .Include(r => r.User).ThenInclude(u => u.Tier)
            .Where(r =>
                r.UserId != _currentUser.UserId &&
                r.Type == oppositeType &&
                r.Currency == query.Currency &&
                r.Amount == query.Amount &&
                r.Status == RequestStatus.Pending)
            .OrderByDescending(r => r.User.IsTrusted)
            .ThenBy(r => r.CreatedAt)
            .ToListAsync(ct);

        return rows.Select(r =>
        {
            var hasFullName = !string.IsNullOrEmpty(r.User.FirstName) && !string.IsNullOrEmpty(r.User.LastName);
            return new MatchingRequestDto(
                r.Id,
                hasFullName ? $"{r.User.FirstName![0]}{r.User.LastName![0]}" : "??",
                hasFullName ? $"{r.User.FirstName} {r.User.LastName![0]}." : "Unknown",
                r.User.Tier.Order,
                r.User.Tier.Name,
                r.User.IsTrusted,
                r.Amount,
                r.RateValue,
                r.PaymentMethods.Select(p => p.ToString()).ToList(),
                r.CreatedAt
            );
        }).ToList();
    }
}
