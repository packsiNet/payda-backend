using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Commands.PreviewRequest;

public class PreviewRequestCommandHandler : IRequestHandler<PreviewRequestCommand, PreviewRequestResult>
{
    private readonly IAppDbContext _context;

    public PreviewRequestCommandHandler(IAppDbContext context) => _context = context;

    public async Task<PreviewRequestResult> Handle(PreviewRequestCommand cmd, CancellationToken ct)
    {
        var exchangeRate = await _context.ExchangeRates
            .FirstOrDefaultAsync(r => r.Currency == cmd.Currency, ct)
            ?? throw new NotFoundException("Exchange rate not found");

        var rateValue = cmd.RateType switch
        {
            RateType.Market => exchangeRate.MarketRate,
            RateType.Instant => exchangeRate.InstantRate,
            RateType.Custom => cmd.CustomRate!.Value,
            _ => throw new ArgumentOutOfRangeException()
        };

        const decimal commissionPercent = 1m;
        var commissionAmount = cmd.Amount * (commissionPercent / 100);
        var totalAmount = cmd.Amount + commissionAmount;

        return new PreviewRequestResult(cmd.Amount, rateValue, commissionPercent, commissionAmount, totalAmount);
    }
}
