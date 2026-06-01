using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Entities;
using PayDa.Domain.Enums;

namespace PayDa.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Tier> Tiers => Set<Tier>();
    public DbSet<Receiver> Receivers => Set<Receiver>();
    public DbSet<Request> Requests => Set<Request>();
    public DbSet<RequestForeignAccount> RequestForeignAccounts => Set<RequestForeignAccount>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
    public DbSet<SystemConfig> SystemConfigs => Set<SystemConfig>();
    public DbSet<TierCommission> TierCommissions => Set<TierCommission>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(builder);

        var bronzeId = new Guid("11111111-1111-1111-1111-111111111111");
        var silverId = new Guid("22222222-2222-2222-2222-222222222222");
        var goldId   = new Guid("33333333-3333-3333-3333-333333333333");
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var adminUserId = new Guid("00000000-0000-0000-0000-000000000001");

        builder.Entity<Tier>().HasData(
            new { Id = bronzeId, Name = "Bronze", Order = 1, MaxActiveRequests = 5,  MaxAmountPerRequest = 200m,   RequiredCompletedTransactions = 0,  CreatedAt = seedDate, UpdatedAt = (DateTime?)null },
            new { Id = silverId, Name = "Silver", Order = 2, MaxActiveRequests = 10, MaxAmountPerRequest = 2000m,  RequiredCompletedTransactions = 5,  CreatedAt = seedDate, UpdatedAt = (DateTime?)null },
            new { Id = goldId,   Name = "Gold",   Order = 3, MaxActiveRequests = -1, MaxAmountPerRequest = 10000m, RequiredCompletedTransactions = 20, CreatedAt = seedDate, UpdatedAt = (DateTime?)null }
        );

        builder.Entity<ExchangeRate>().HasData(
            new { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Currency = Currency.EUR, MarketRate = 65000m, InstantRate = 64000m, UpdatedByUserId = adminUserId, CreatedAt = seedDate, UpdatedAt = (DateTime?)null },
            new { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Currency = Currency.USD, MarketRate = 60000m, InstantRate = 59000m, UpdatedByUserId = adminUserId, CreatedAt = seedDate, UpdatedAt = (DateTime?)null },
            new { Id = new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), Currency = Currency.CAD, MarketRate = 44000m, InstantRate = 43000m, UpdatedByUserId = adminUserId, CreatedAt = seedDate, UpdatedAt = (DateTime?)null }
        );

        builder.Entity<SystemConfig>().HasData(
            new { Id = new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), MatchConfirmationHours = 24, CreatedAt = seedDate, UpdatedAt = (DateTime?)null }
        );
    }
}
