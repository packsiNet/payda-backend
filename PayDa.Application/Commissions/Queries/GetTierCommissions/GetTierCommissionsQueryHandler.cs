using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Commissions.Queries.GetTierCommissions;

public class GetTierCommissionsQueryHandler : IRequestHandler<GetTierCommissionsQuery, List<TierCommissionDto>>
{
    private readonly IAppDbContext _context;

    public GetTierCommissionsQueryHandler(IAppDbContext context) => _context = context;

    public async Task<List<TierCommissionDto>> Handle(GetTierCommissionsQuery request, CancellationToken ct)
    {
        return await _context.TierCommissions
            .Include(c => c.Tier)
            .OrderBy(c => c.Tier.Order)
            .Select(c => new TierCommissionDto(
                c.Id,
                c.TierId,
                c.Tier.Name,
                c.Tier.Order,
                c.SenderCommissionPercent,
                c.ReceiverCommissionPercent
            ))
            .ToListAsync(ct);
    }
}
