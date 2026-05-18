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

        var oppositeType = otherRequest.Type == RequestType.Send ? RequestType.Receive : RequestType.Send;

        var myRequest = await _context.Requests
            .Where(r =>
                r.UserId == userId &&
                r.Type == oppositeType &&
                r.Currency == otherRequest.Currency &&
                r.Amount == otherRequest.Amount &&
                r.Status == RequestStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("No matching pending request found for your account");

        var senderRequestId = myRequest.Type == RequestType.Send ? myRequest.Id : otherRequest.Id;
        var receiverRequestId = myRequest.Type == RequestType.Receive ? myRequest.Id : otherRequest.Id;

        var match = Match.Create(senderRequestId, receiverRequestId, isAgentInvolved: false);
        _context.Matches.Add(match);

        myRequest.SetMatched();
        otherRequest.SetMatched();

        var transaction = Transaction.Create(match.Id);
        _context.Transactions.Add(transaction);

        await _context.SaveChangesAsync(ct);

        return new CreateUserMatchResult(match.Id, "Match created successfully");
    }
}
