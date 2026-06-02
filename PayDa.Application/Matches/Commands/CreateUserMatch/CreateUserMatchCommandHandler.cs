using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Entities;
using PayDa.Domain.Enums;

namespace PayDa.Application.Matches.Commands.CreateUserMatch;

public class CreateUserMatchCommandHandler : IRequestHandler<CreateUserMatchCommand, CreateUserMatchResult>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateUserMatchCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CreateUserMatchResult> Handle(CreateUserMatchCommand cmd, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var otherRequest = await _context.Requests
            .FirstOrDefaultAsync(r => r.Id == cmd.RequestId && r.Status == RequestStatus.Pending, ct)
            ?? throw new NotFoundException("Request not found or no longer available");

        if (otherRequest.UserId == userId)
            throw new ForbiddenException("Cannot match with your own request");

        var myType = otherRequest.Type == RequestType.Send ? RequestType.Receive : RequestType.Send;

        var myRequest = await _context.Requests
            .Where(r =>
                r.UserId == userId &&
                r.Type == myType &&
                r.Currency == otherRequest.Currency &&
                r.Amount == otherRequest.Amount &&
                r.Status == RequestStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (myRequest is null)
            myRequest = myType == RequestType.Receive
                ? await AutoCreateReceiveRequestAsync(cmd, userId, otherRequest, ct)
                : await AutoCreateSendRequestAsync(cmd, userId, otherRequest, ct);

        var senderRequestId = myRequest.Type == RequestType.Send ? myRequest.Id : otherRequest.Id;
        var receiverRequestId = myRequest.Type == RequestType.Receive ? myRequest.Id : otherRequest.Id;

        var match = Match.Create(senderRequestId, receiverRequestId, price: 0, isAgentInvolved: false);
        _context.Matches.Add(match);

        myRequest.SetMatched(match.Id);
        otherRequest.SetMatched(match.Id);

        _context.Transactions.Add(Transaction.Create(match.Id));

        await _context.SaveChangesAsync(ct);

        return new CreateUserMatchResult(match.Id, "Match created successfully");
    }

    private async Task<Request> AutoCreateReceiveRequestAsync(
        CreateUserMatchCommand cmd, Guid userId, Request otherRequest, CancellationToken ct)
    {
        if (cmd.ForeignAccounts is not { Count: > 0 })
            throw new BadRequestException("ForeignAccounts are required to create a Receive request");

        await ValidateTierAsync(userId, otherRequest.Amount, ct);

        var request = Request.Create(
            userId: userId,
            type: RequestType.Receive,
            currency: otherRequest.Currency,
            amount: otherRequest.Amount,
            pricePreference: otherRequest.PricePreference,
            paymentMethods: otherRequest.PaymentMethods,
            receiverId: null,
            expiresAt: DateTime.UtcNow.AddHours(24)
        );
        _context.Requests.Add(request);
        await _context.SaveChangesAsync(ct);

        foreach (var fa in cmd.ForeignAccounts)
        {
            _context.RequestForeignAccounts.Add(RequestForeignAccount.Create(
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
            ));
        }
        await _context.SaveChangesAsync(ct);

        return request;
    }

    private async Task<Request> AutoCreateSendRequestAsync(
        CreateUserMatchCommand cmd, Guid userId, Request otherRequest, CancellationToken ct)
    {
        if (cmd.ReceiverInfo is null)
            throw new BadRequestException("ReceiverInfo is required to create a Send request");

        await ValidateTierAsync(userId, otherRequest.Amount, ct);

        var receiver = Receiver.Create(
            userId: userId,
            firstName: cmd.ReceiverInfo.FirstName,
            lastName: cmd.ReceiverInfo.LastName,
            nationalId: cmd.ReceiverInfo.NationalId,
            mobileNumber: cmd.ReceiverInfo.MobileNumber,
            iban: cmd.ReceiverInfo.IBAN
        );
        _context.Receivers.Add(receiver);
        await _context.SaveChangesAsync(ct);

        var request = Request.Create(
            userId: userId,
            type: RequestType.Send,
            currency: otherRequest.Currency,
            amount: otherRequest.Amount,
            pricePreference: otherRequest.PricePreference,
            paymentMethods: otherRequest.PaymentMethods,
            receiverId: receiver.Id,
            expiresAt: DateTime.UtcNow.AddHours(24)
        );
        _context.Requests.Add(request);
        await _context.SaveChangesAsync(ct);

        return request;
    }

    private async Task ValidateTierAsync(Guid userId, decimal amount, CancellationToken ct)
    {
        var user = await _context.Users
            .Include(u => u.Tier)
            .FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new UnauthorizedException("Session expired, please login again");

        if (user.KycStatus != KycStatus.Approved)
            throw new ForbiddenException("KYC not approved");

        var activeCount = await _context.Requests
            .CountAsync(r => r.UserId == userId && r.Status == RequestStatus.Pending, ct);

        if (user.Tier.MaxActiveRequests != -1 && activeCount >= user.Tier.MaxActiveRequests)
            throw new ForbiddenException("Request limit reached for your tier");

        if (amount > user.Tier.MaxAmountPerRequest)
            throw new ForbiddenException($"Amount exceeds tier limit of {user.Tier.MaxAmountPerRequest}");
    }
}
