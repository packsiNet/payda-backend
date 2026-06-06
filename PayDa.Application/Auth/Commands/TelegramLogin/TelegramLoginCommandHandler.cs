using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Entities;

namespace PayDa.Application.Auth.Commands.TelegramLogin;

public class TelegramLoginCommandHandler : IRequestHandler<TelegramLoginCommand, TelegramLoginResult>
{
    private readonly IAppDbContext _context;
    private readonly ITelegramAuthService _telegramAuth;
    private readonly IJwtService _jwtService;

    public TelegramLoginCommandHandler(IAppDbContext context,
        ITelegramAuthService telegramAuth, IJwtService jwtService)
    {
        _context = context;
        _telegramAuth = telegramAuth;
        _jwtService = jwtService;
    }

    public async Task<TelegramLoginResult> Handle(TelegramLoginCommand request, CancellationToken ct)
    {
        if (!_telegramAuth.ValidateInitData(request.InitData))
            throw new UnauthorizedException("Invalid Telegram initData");

        var telegramUser = _telegramAuth.ParseInitData(request.InitData);

        var user = await _context.Users
            .Include(u => u.Tier)
            .FirstOrDefaultAsync(u => u.TelegramId == telegramUser.Id, ct);

        bool isNewUser = user is null;

        if (isNewUser)
        {
            var bronzeTier = await _context.Tiers
                .OrderBy(t => t.Order)
                .FirstAsync(ct);

            user = User.Create(telegramUser.Id, telegramUser.Username, telegramUser.FirstName, telegramUser.LastName, telegramUser.PhotoUrl);
            user.UpgradeTier(bronzeTier.Id);

            if (!string.IsNullOrWhiteSpace(request.ReferralCode))
            {
                var referrer = await _context.Users
                    .FirstOrDefaultAsync(u => u.ReferralCode == request.ReferralCode.ToUpper(), ct);
                if (referrer is not null && referrer.TelegramId != telegramUser.Id)
                    user.ApplyReferral(referrer.Id);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);

            user = await _context.Users
                .Include(u => u.Tier)
                .FirstAsync(u => u.Id == user.Id, ct);
        }
        else
        {
            user!.UpdateTelegramProfile(telegramUser.Username, telegramUser.FirstName, telegramUser.LastName, telegramUser.PhotoUrl);
            await _context.SaveChangesAsync(ct);
        }

        var token = _jwtService.GenerateToken(user!);
        return new TelegramLoginResult(
            token,
            isNewUser,
            user.Id,
            user.TelegramId,
            user.TelegramUsername,
            user.FirstName,
            user.LastName,
            user.KycStatus,
            user.Role,
            user.IsTrusted,
            user.Tier.Name,
            user.Tier.Order,
            user.CompletedTransactionsCount,
            user.PhoneNumber != null,
            user.SelfieImageUrl,
            user.DocumentImageUrl,
            user.ReferralCode
        );
    }
}
