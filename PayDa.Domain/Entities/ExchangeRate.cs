using PayDa.Domain.Common;
using PayDa.Domain.Enums;

namespace PayDa.Domain.Entities;

public class ExchangeRate : BaseEntity
{
    public Currency Currency { get; private set; }
    public decimal MarketRate { get; private set; }
    public decimal InstantRate { get; private set; }
    public Guid UpdatedByUserId { get; private set; }

    private ExchangeRate() { }

    public static ExchangeRate Create(Currency currency, decimal marketRate,
        decimal instantRate, Guid updatedByUserId) => new()
    {
        Currency = currency,
        MarketRate = marketRate,
        InstantRate = instantRate,
        UpdatedByUserId = updatedByUserId
    };

    public void Update(decimal marketRate, decimal instantRate, Guid updatedByUserId)
    {
        MarketRate = marketRate;
        InstantRate = instantRate;
        UpdatedByUserId = updatedByUserId;
        UpdatedAt = DateTime.UtcNow;
    }
}
