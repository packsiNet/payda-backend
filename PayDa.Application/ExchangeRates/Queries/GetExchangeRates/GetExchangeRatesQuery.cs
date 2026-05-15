using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.ExchangeRates.Queries.GetExchangeRates;

public record GetExchangeRatesQuery : IRequest<List<ExchangeRateDto>>;

public record ExchangeRateDto(Currency Currency, decimal MarketRate, decimal InstantRate, DateTime? UpdatedAt);
