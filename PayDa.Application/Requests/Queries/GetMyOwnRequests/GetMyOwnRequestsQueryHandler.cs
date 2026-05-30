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
        var query = _context.Requests
            .Include(r => r.Match)
                .ThenInclude(m => m!.Transaction)
            .Where(r => r.UserId == _currentUser.UserId)
            .Where(r => r.Status != RequestStatus.Cancelled)
            .Where(r => r.Status != RequestStatus.Expired)
            .Where(r => !(r.Status == RequestStatus.Matched &&
                          r.Match != null &&
                          r.Match.Transaction != null &&
                          r.Match.Transaction.Status == TransactionStatus.Completed))
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
