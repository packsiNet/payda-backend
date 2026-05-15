using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.ExchangeRates.Commands.UpdateExchangeRate;

public record UpdateExchangeRateCommand(Currency Currency, decimal MarketRate, decimal InstantRate) : IRequest;
