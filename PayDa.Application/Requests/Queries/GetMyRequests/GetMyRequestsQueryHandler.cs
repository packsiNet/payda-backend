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

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RequestSummaryDto(
                r.Id,
                r.Type,
                r.Currency,
                r.Amount,
                r.RateValue,
                r.PaymentMethods.Select(p => p.ToString()).ToList(),
                r.Status,
                r.ExpiresAt,
                r.CreatedAt,
                r.User.FirstName != null && r.User.LastName != null
                    ? $"{r.User.FirstName[0]}{r.User.LastName[0]}"
                    : r.User.TelegramUsername != null ? r.User.TelegramUsername.Substring(0, 1).ToUpper() : null,
                r.User.ProfilePhotoUrl,
                r.User.FirstName != null && r.User.LastName != null
                    ? $"{r.User.FirstName} {r.User.LastName}"
                    : r.User.TelegramUsername,
                r.User.IsTrusted,
                r.User.Tier.Name,
                r.User.Tier.Order
            ))
            .ToListAsync(ct);
    }
}
