using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Tiers.Queries.GetAllTiers;

public class GetAllTiersQueryHandler : IRequestHandler<GetAllTiersQuery, List<TierDto>>
{
    private readonly IAppDbContext _context;

    public GetAllTiersQueryHandler(IAppDbContext context) => _context = context;

    public async Task<List<TierDto>> Handle(GetAllTiersQuery request, CancellationToken ct)
    {
        return await _context.Tiers
            .OrderBy(t => t.Order)
            .Select(t => new TierDto(t.Id, t.Name, t.Order, t.MaxActiveRequests,
                t.MaxAmountPerRequest, t.RequiredCompletedTransactions))
            .ToListAsync(ct);
    }
}
