using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Requests.Queries.GetMyRequests;

public class GetMyRequestsQueryHandler : IRequestHandler<GetMyRequestsQuery, List<RequestSummaryDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyRequestsQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<RequestSummaryDto>> Handle(GetMyRequestsQuery request, CancellationToken ct)
    {
        var query = _context.Requests.Where(r => r.UserId == _currentUser.UserId);

        if (request.Type.HasValue)
            query = query.Where(r => r.Type == request.Type.Value);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RequestSummaryDto(
                r.Id, r.Type, r.Currency, r.Amount,
                r.RateValue, r.Status, r.ExpiresAt, r.CreatedAt))
            .ToListAsync(ct);
    }
}
