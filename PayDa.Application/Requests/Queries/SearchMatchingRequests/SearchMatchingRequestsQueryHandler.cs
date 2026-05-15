using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Queries.SearchMatchingRequests;

public class SearchMatchingRequestsQueryHandler
    : IRequestHandler<SearchMatchingRequestsQuery, List<MatchingRequestDto>>
{
    private readonly IAppDbContext _context;

    public SearchMatchingRequestsQueryHandler(IAppDbContext context) => _context = context;

    public async Task<List<MatchingRequestDto>> Handle(
        SearchMatchingRequestsQuery query, CancellationToken ct)
    {
        var oppositeType = query.Type == RequestType.Send ? RequestType.Receive : RequestType.Send;

        return await _context.Requests
            .Include(r => r.User).ThenInclude(u => u.Tier)
            .Where(r =>
                r.Type == oppositeType &&
                r.Currency == query.Currency &&
                r.Amount == query.Amount &&
                r.Status == RequestStatus.Pending)
            .OrderByDescending(r => r.User.IsTrusted)
            .ThenBy(r => r.CreatedAt)
            .Select(r => new MatchingRequestDto(
                r.Id,
                r.User.FirstName != null && r.User.LastName != null
                    ? $"{r.User.FirstName[0]}{r.User.LastName[0]}"
                    : "??",
                r.User.FirstName != null && r.User.LastName != null
                    ? $"{r.User.FirstName} {r.User.LastName[0]}."
                    : "Unknown",
                r.User.Tier.Order,
                r.User.Tier.Name,
                r.User.IsTrusted,
                r.Amount,
                r.RateValue,
                r.PaymentMethods.Select(p => p.ToString()).ToList(),
                r.CreatedAt
            ))
            .ToListAsync(ct);
    }
}
