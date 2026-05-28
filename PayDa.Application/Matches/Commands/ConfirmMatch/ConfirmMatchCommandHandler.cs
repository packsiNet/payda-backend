using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Entities;
using PayDa.Domain.Enums;

namespace PayDa.Application.Matches.Commands.ConfirmMatch;

public class ConfirmMatchCommandHandler : IRequestHandler<ConfirmMatchCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ConfirmMatchCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(ConfirmMatchCommand cmd, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var match = await _context.Matches
            .Include(m => m.SenderRequest)
            .Include(m => m.ReceiverRequest)
            .FirstOrDefaultAsync(m => m.Id == cmd.MatchId && m.Status == MatchStatus.PendingConfirmation, ct)
            ?? throw new NotFoundException("Pending match not found");

        if (match.ConfirmationDeadline < DateTime.UtcNow)
            throw new BadRequestException("Confirmation deadline has passed");

        bool bothConfirmed;
        if (match.SenderRequest.UserId == userId)
            bothConfirmed = match.ConfirmBySender();
        else if (match.ReceiverRequest.UserId == userId)
            bothConfirmed = match.ConfirmByReceiver();
        else
            throw new ForbiddenException("You are not a participant in this match");

        if (bothConfirmed)
        {
            var transaction = Transaction.Create(match.Id);
            _context.Transactions.Add(transaction);
        }

        await _context.SaveChangesAsync(ct);
    }
}
