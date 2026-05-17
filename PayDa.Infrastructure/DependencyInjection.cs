using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayDa.Application.Common.Interfaces;
using PayDa.Infrastructure.Persistence;
using PayDa.Infrastructure.Services;

namespace PayDa.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAppDbContext>(p => p.GetRequiredService<AppDbContext>());
        services.AddScoped<ITelegramAuthService, TelegramAuthService>();
        services.AddScoped<IStorageService, StorageService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}
