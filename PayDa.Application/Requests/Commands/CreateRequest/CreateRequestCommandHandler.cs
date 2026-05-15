using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Entities;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Commands.CreateRequest;

public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, Guid>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateRequestCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateRequestCommand cmd, CancellationToken ct)
    {
        var user = await _context.Users
            .Include(u => u.Tier)
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, ct)
            ?? throw new NotFoundException("User not found");

        if (user.KycStatus != KycStatus.Approved)
            throw new ForbiddenException("KYC not approved");

        var activeRequestsCount = await _context.Requests
            .CountAsync(r => r.UserId == user.Id && r.Status == RequestStatus.Pending, ct);

        if (user.Tier.MaxActiveRequests != -1 && activeRequestsCount >= user.Tier.MaxActiveRequests)
            throw new ForbiddenException("Request limit reached for your tier");

        if (cmd.Amount > user.Tier.MaxAmountPerRequest)
            throw new ForbiddenException($"Amount exceeds tier limit of {user.Tier.MaxAmountPerRequest}");

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

        var request = Request.Create(
            userId: user.Id,
            type: cmd.Type,
            currency: cmd.Currency,
            amount: cmd.Amount,
            rateType: cmd.RateType,
            rateValue: rateValue,
            commissionPercent: commissionPercent,
            paymentMethods: cmd.PaymentMethods,
            receiverId: cmd.ReceiverId,
            expiresAt: DateTime.UtcNow.AddHours(24)
        );

        _context.Requests.Add(request);
        await _context.SaveChangesAsync(ct);
        return request.Id;
    }
}
