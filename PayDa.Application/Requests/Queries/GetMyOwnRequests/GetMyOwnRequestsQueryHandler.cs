using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Queries.GetMyOwnRequests;

public class GetMyOwnRequestsQueryHandler : IRequestHandler<GetMyOwnRequestsQuery, List<MyOwnRequestDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyOwnRequestsQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<MyOwnRequestDto>> Handle(GetMyOwnRequestsQuery request, CancellationToken ct)
    {
        var completedMatchIds = _context.Transactions
            .Where(t => t.Status == TransactionStatus.Completed)
            .Select(t => t.MatchId);

        var query = _context.Requests
            .Where(r => r.UserId == _currentUser.UserId)
            .Where(r => r.Status != RequestStatus.Cancelled)
            .Where(r => r.Status != RequestStatus.Expired)
            .Where(r => !(r.Status == RequestStatus.Matched &&
                          r.MatchId.HasValue &&
                          completedMatchIds.Contains(r.MatchId.Value)))
            .AsQueryable();

        if (request.Type.HasValue)
            query = query.Where(r => r.Type == request.Type.Value);

        var rows = await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        return [.. rows.Select(r => new MyOwnRequestDto(
            r.Id,
            r.Type,
            r.Currency,
            r.Amount,
            r.PricePreference,
            [.. r.PaymentMethods.Select(p => p.ToString())],
            r.Status,
            r.MatchId,
            r.ExpiresAt,
            r.CreatedAt
        ))];
    }
}
