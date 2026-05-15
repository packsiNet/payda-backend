using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Entities;

namespace PayDa.Application.ExchangeRates.Commands.UpdateExchangeRate;

public class UpdateExchangeRateCommandHandler : IRequestHandler<UpdateExchangeRateCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateExchangeRateCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateExchangeRateCommand cmd, CancellationToken ct)
    {
        var rate = await _context.ExchangeRates
            .FirstOrDefaultAsync(r => r.Currency == cmd.Currency, ct);

        if (rate is null)
        {
            rate = ExchangeRate.Create(cmd.Currency, cmd.MarketRate, cmd.InstantRate, _currentUser.UserId);
            _context.ExchangeRates.Add(rate);
        }
        else
        {
            rate.Update(cmd.MarketRate, cmd.InstantRate, _currentUser.UserId);
        }

        await _context.SaveChangesAsync(ct);
    }
}
