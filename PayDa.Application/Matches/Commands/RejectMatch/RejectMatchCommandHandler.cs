using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Matches.Commands.RejectMatch;

public class RejectMatchCommandHandler : IRequestHandler<RejectMatchCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RejectMatchCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(RejectMatchCommand cmd, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var match = await _context.Matches
            .Include(m => m.SenderRequest)
            .Include(m => m.ReceiverRequest)
            .FirstOrDefaultAsync(m => m.Id == cmd.MatchId && m.Status == MatchStatus.PendingConfirmation, ct)
            ?? throw new NotFoundException("Pending match not found");

        if (match.SenderRequest.UserId != userId && match.ReceiverRequest.UserId != userId)
            throw new ForbiddenException("You are not a participant in this match");

        match.Reject();
        match.SenderRequest.ResetToPending();
        match.ReceiverRequest.ResetToPending();

        await _context.SaveChangesAsync(ct);
    }
}
