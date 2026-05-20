using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Entities;
using PayDa.Domain.Enums;

namespace PayDa.Application.Matches.Commands.MatchRequests;

public class MatchRequestsCommandHandler : IRequestHandler<MatchRequestsCommand, Guid>
{
    private readonly IAppDbContext _context;

    public MatchRequestsCommandHandler(IAppDbContext context) => _context = context;

    public async Task<Guid> Handle(MatchRequestsCommand cmd, CancellationToken ct)
    {
        var senderRequest = await _context.Requests
            .FirstOrDefaultAsync(r => r.Id == cmd.SenderRequestId && r.Status == RequestStatus.Pending, ct)
            ?? throw new NotFoundException("Sender request not found or not pending");

        var receiverRequest = await _context.Requests
            .FirstOrDefaultAsync(r => r.Id == cmd.ReceiverRequestId && r.Status == RequestStatus.Pending, ct)
            ?? throw new NotFoundException("Receiver request not found or not pending");

        var match = Match.Create(cmd.SenderRequestId, cmd.ReceiverRequestId, cmd.IsAgentInvolved);
        _context.Matches.Add(match);

        senderRequest.SetMatched(match.Id);
        receiverRequest.SetMatched(match.Id);

        var transaction = Transaction.Create(match.Id);
        _context.Transactions.Add(transaction);

        await _context.SaveChangesAsync(ct);
        return match.Id;
    }
}
