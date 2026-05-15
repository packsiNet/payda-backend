using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.ExchangeRates.Queries.GetExchangeRates;

public class GetExchangeRatesQueryHandler : IRequestHandler<GetExchangeRatesQuery, List<ExchangeRateDto>>
{
    private readonly IAppDbContext _context;

    public GetExchangeRatesQueryHandler(IAppDbContext context) => _context = context;

    public async Task<List<ExchangeRateDto>> Handle(GetExchangeRatesQuery request, CancellationToken ct)
    {
        return await _context.ExchangeRates
            .Select(r => new ExchangeRateDto(r.Currency, r.MarketRate, r.InstantRate, r.UpdatedAt))
            .ToListAsync(ct);
    }
}
