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
    private readonly ITelegramBotService _telegramBot;

    public MatchRequestsCommandHandler(IAppDbContext context, ITelegramBotService telegramBot)
    {
        _context = context;
        _telegramBot = telegramBot;
    }

    public async Task<Guid> Handle(MatchRequestsCommand cmd, CancellationToken ct)
    {
        var senderRequest = await _context.Requests
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == cmd.SenderRequestId && r.Status == RequestStatus.Pending, ct)
            ?? throw new NotFoundException("Sender request not found or not pending");

        var receiverRequest = await _context.Requests
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == cmd.ReceiverRequestId && r.Status == RequestStatus.Pending, ct)
            ?? throw new NotFoundException("Receiver request not found or not pending");

        var config = await _context.SystemConfigs.FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("System config not found");

        var confirmationDeadline = DateTime.UtcNow.AddHours(config.MatchConfirmationHours);

        var match = Match.CreatePending(
            cmd.SenderRequestId, cmd.ReceiverRequestId,
            cmd.Price, cmd.IsAgentInvolved, confirmationDeadline);

        _context.Matches.Add(match);

        senderRequest.SetMatched(match.Id);
        receiverRequest.SetMatched(match.Id);

        await _context.SaveChangesAsync(ct);

        await _telegramBot.SendMatchNotificationAsync(
            senderRequest.User.TelegramId,
            GetDisplayName(senderRequest.User),
            senderRequest.Type,
            cmd.Price, ct);

        await _telegramBot.SendMatchNotificationAsync(
            receiverRequest.User.TelegramId,
            GetDisplayName(receiverRequest.User),
            receiverRequest.Type,
            cmd.Price, ct);

        return match.Id;
    }

    private static string GetDisplayName(User user)
    {
        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        if (!string.IsNullOrWhiteSpace(fullName)) return fullName;
        if (!string.IsNullOrWhiteSpace(user.TelegramUsername)) return $"@{user.TelegramUsername}";
        return user.TelegramId.ToString();
    }
}
