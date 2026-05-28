using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Queries.GetAdminRequests;

public class GetAdminRequestsQueryHandler : IRequestHandler<GetAdminRequestsQuery, List<AdminRequestDto>>
{
    private readonly IAppDbContext _context;

    public GetAdminRequestsQueryHandler(IAppDbContext context) => _context = context;

    public async Task<List<AdminRequestDto>> Handle(GetAdminRequestsQuery query, CancellationToken ct)
    {
        var q = _context.Requests
            .Include(r => r.User).ThenInclude(u => u.Tier)
            .Where(r =>
                r.Type == query.Type &&
                r.Currency == query.Currency &&
                r.Status == RequestStatus.Pending);

        if (query.Amount.HasValue)
            q = q.Where(r => r.Amount == query.Amount.Value);

        var rows = await q
            .OrderByDescending(r => r.User.IsTrusted)
            .ThenBy(r => r.CreatedAt)
            .ToListAsync(ct);

        return [.. rows.Select(r =>
        {
            var hasFullName = !string.IsNullOrEmpty(r.User.FirstName) && !string.IsNullOrEmpty(r.User.LastName);
            return new AdminRequestDto(
                r.Id,
                r.Type,
                r.Currency,
                r.Amount,
                r.PricePreference,
                [.. r.PaymentMethods.Select(p => p.ToString())],
                r.ExpiresAt,
                r.CreatedAt,
                hasFullName ? $"{r.User.FirstName} {r.User.LastName}" : r.User.TelegramUsername ?? "Unknown",
                hasFullName ? $"{r.User.FirstName![0]}{r.User.LastName![0]}" : "??",
                r.User.Tier.Order,
                r.User.Tier.Name,
                r.User.IsTrusted
            );
        })];
    }
}
