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
            ?? throw new UnauthorizedException("Session expired, please login again");

        if (user.KycStatus != KycStatus.Approved)
            throw new ForbiddenException("KYC not approved");

        var activeRequestsCount = await _context.Requests
            .CountAsync(r => r.UserId == user.Id && r.Status == RequestStatus.Pending, ct);

        if (user.Tier.MaxActiveRequests != -1 && activeRequestsCount >= user.Tier.MaxActiveRequests)
            throw new ForbiddenException("Request limit reached for your tier");

        if (cmd.Amount > user.Tier.MaxAmountPerRequest)
            throw new ForbiddenException($"Amount exceeds tier limit of {user.Tier.MaxAmountPerRequest}");

        Guid? receiverId = null;

        if (cmd.Type == RequestType.Send)
        {
            if (cmd.ReceiverId.HasValue)
            {
                var receiverExists = await _context.Receivers
                    .AnyAsync(r => r.Id == cmd.ReceiverId.Value && r.UserId == user.Id, ct);
                if (!receiverExists)
                    throw new NotFoundException("Receiver not found");

                receiverId = cmd.ReceiverId.Value;
            }
            else if (cmd.NewReceiver is not null)
            {
                var receiver = Receiver.Create(
                    user.Id,
                    cmd.NewReceiver.FirstName,
                    cmd.NewReceiver.LastName,
                    cmd.NewReceiver.NationalId,
                    cmd.NewReceiver.MobileNumber,
                    cmd.NewReceiver.IBAN
                );
                _context.Receivers.Add(receiver);
                await _context.SaveChangesAsync(ct);
                receiverId = receiver.Id;
            }
            else
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "ReceiverId", ["Either ReceiverId or NewReceiver is required for Send requests."] }
                });
            }
        }

        var request = Request.Create(
            userId: user.Id,
            type: cmd.Type,
            currency: cmd.Currency,
            amount: cmd.Amount,
            pricePreference: cmd.PricePreference,
            paymentMethods: cmd.PaymentMethods,
            receiverId: receiverId,
            expiresAt: DateTime.UtcNow.AddHours(24),
            tomanPayerFullName: cmd.TomanPayer?.FullName,
            tomanPayerMobileNumber: cmd.TomanPayer?.MobileNumber
        );

        _context.Requests.Add(request);
        await _context.SaveChangesAsync(ct);

        if (cmd.Type == RequestType.Receive && cmd.ForeignAccounts is { Count: > 0 })
        {
            foreach (var fa in cmd.ForeignAccounts)
            {
                var account = RequestForeignAccount.Create(
                    requestId: request.Id,
                    method: fa.Method,
                    fullName: fa.FullName,
                    username: fa.Username,
                    email: fa.Email,
                    emailOrPhone: fa.EmailOrPhone,
                    iban: fa.Iban,
                    bic: fa.Bic,
                    bankName: fa.BankName,
                    accountNum: fa.AccountNum,
                    swift: fa.Swift,
                    bankAddress: fa.BankAddress
                );
                _context.RequestForeignAccounts.Add(account);
            }
            await _context.SaveChangesAsync(ct);
        }

        return request.Id;
    }
}
