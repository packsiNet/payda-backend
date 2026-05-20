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
        var query = _context.Requests
            .Include(r => r.Match)
                .ThenInclude(m => m!.SenderRequest)
                    .ThenInclude(sr => sr.User)
                        .ThenInclude(u => u.Tier)
            .Include(r => r.Match)
                .ThenInclude(m => m!.ReceiverRequest)
                    .ThenInclude(rr => rr.User)
                        .ThenInclude(u => u.Tier)
            .Where(r => r.UserId == _currentUser.UserId);

        if (request.Type.HasValue)
            query = query.Where(r => r.Type == request.Type.Value);

        var rows = await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        return rows.Select(r =>
        {
            var counterpartUser = r.Match == null ? null
                : r.Match.SenderRequestId == r.Id
                    ? r.Match.ReceiverRequest.User
                    : r.Match.SenderRequest.User;

            string? initials = null;
            string? displayName = null;
            if (counterpartUser != null)
            {
                initials = counterpartUser.FirstName != null && counterpartUser.LastName != null
                    ? $"{counterpartUser.FirstName[0]}{counterpartUser.LastName[0]}"
                    : counterpartUser.TelegramUsername?[..1]?.ToUpper();

                displayName = counterpartUser.FirstName != null && counterpartUser.LastName != null
                    ? $"{counterpartUser.FirstName} {counterpartUser.LastName}"
                    : counterpartUser.TelegramUsername;
            }

            return new RequestSummaryDto(
                r.Id,
                r.Type,
                r.Currency,
                r.Amount,
                r.RateValue,
                r.PaymentMethods.Select(p => p.ToString()).ToList(),
                r.Status,
                r.ExpiresAt,
                r.CreatedAt,
                initials,
                counterpartUser?.ProfilePhotoUrl,
                displayName,
                counterpartUser?.IsTrusted,
                counterpartUser?.Tier.Name,
                counterpartUser?.Tier.Order
            );
        }).ToList();
    }
}
