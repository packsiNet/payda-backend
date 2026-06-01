using Microsoft.EntityFrameworkCore;
using PayDa.Domain.Entities;

namespace PayDa.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Tier> Tiers { get; }
    DbSet<Receiver> Receivers { get; }
    DbSet<Request> Requests { get; }
    DbSet<RequestForeignAccount> RequestForeignAccounts { get; }
    DbSet<Match> Matches { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<ExchangeRate> ExchangeRates { get; }
    DbSet<SystemConfig> SystemConfigs { get; }
    DbSet<TierCommission> TierCommissions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
