using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Queries.GetMyRequests;

public class GetMyRequestsQueryHandler : IRequestHandler<GetMyRequestsQuery, List<RequestSummaryDto>>
{
    private readonly IAppDbContext _context;

    public GetMyRequestsQueryHandler(IAppDbContext context) => _context = context;

    public async Task<List<RequestSummaryDto>> Handle(GetMyRequestsQuery request, CancellationToken ct)
    {
        var query = _context.Requests
            .Include(r => r.User).ThenInclude(u => u.Tier)
            .AsQueryable();

        if (request.Type.HasValue)
            query = query.Where(r => r.Type == request.Type.Value);

        var rows = await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        return [.. rows.Select(r =>
        {
            var hasFullName = !string.IsNullOrEmpty(r.User.FirstName) && !string.IsNullOrEmpty(r.User.LastName);
            return new RequestSummaryDto(
                r.Id,
                r.Type,
                r.Currency,
                r.Amount,
                r.RateValue,
                [.. r.PaymentMethods.Select(p => p.ToString())],
                r.Status,
                r.ExpiresAt,
                r.CreatedAt,
                hasFullName
                    ? $"{r.User.FirstName![0]}{r.User.LastName![0]}"
                    : r.User.TelegramUsername?[..1]?.ToUpper(),
                r.User.ProfilePhotoUrl,
                hasFullName
                    ? $"{r.User.FirstName} {r.User.LastName}"
                    : r.User.TelegramUsername,
                r.User.IsTrusted,
                r.User.Tier.Name,
                r.User.Tier.Order
            );
        })];
    }
}
