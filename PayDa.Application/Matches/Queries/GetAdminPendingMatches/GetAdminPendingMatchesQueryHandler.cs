using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Matches.Queries.GetAdminPendingMatches;

public class GetAdminPendingMatchesQueryHandler
    : IRequestHandler<GetAdminPendingMatchesQuery, List<AdminPendingMatchDto>>
{
    private readonly IAppDbContext _context;

    public GetAdminPendingMatchesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminPendingMatchDto>> Handle(
        GetAdminPendingMatchesQuery request, CancellationToken ct)
    {
        var matches = await _context.Matches
            .Include(m => m.SenderRequest).ThenInclude(r => r.User)
            .Include(m => m.ReceiverRequest).ThenInclude(r => r.User)
            .Where(m => m.Status == MatchStatus.PendingConfirmation)
            .OrderBy(m => m.ConfirmationDeadline)
            .ToListAsync(ct);

        var now = DateTime.UtcNow;

        return matches.Select(m =>
        {
            var sender = m.SenderRequest.User;
            var receiver = m.ReceiverRequest.User;

            var senderHasFullName = !string.IsNullOrEmpty(sender.FirstName) && !string.IsNullOrEmpty(sender.LastName);
            var receiverHasFullName = !string.IsNullOrEmpty(receiver.FirstName) && !string.IsNullOrEmpty(receiver.LastName);

            var remaining = m.ConfirmationDeadline!.Value - now;

            return new AdminPendingMatchDto(
                m.Id,
                m.SenderRequest.Amount,
                m.SenderRequest.Currency.ToString(),
                m.Price,
                m.PriceSetAt!.Value,
                m.ConfirmationDeadline!.Value,
                (long)Math.Max(0, remaining.TotalSeconds),
                new AdminPendingMatchPartyDto(
                    sender.Id,
                    senderHasFullName ? $"{sender.FirstName} {sender.LastName}" : sender.TelegramUsername ?? "Unknown",
                    sender.IsTrusted,
                    m.SenderConfirmed
                ),
                new AdminPendingMatchPartyDto(
                    receiver.Id,
                    receiverHasFullName ? $"{receiver.FirstName} {receiver.LastName}" : receiver.TelegramUsername ?? "Unknown",
                    receiver.IsTrusted,
                    m.ReceiverConfirmed
                )
            );
        }).ToList();
    }
}
