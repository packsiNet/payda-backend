using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Matches.Queries.GetPendingConfirmationMatches;

public class GetPendingConfirmationMatchesQueryHandler
    : IRequestHandler<GetPendingConfirmationMatchesQuery, List<PendingConfirmationMatchDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPendingConfirmationMatchesQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<PendingConfirmationMatchDto>> Handle(
        GetPendingConfirmationMatchesQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var matches = await _context.Matches
            .Include(m => m.SenderRequest).ThenInclude(r => r.User)
            .Include(m => m.ReceiverRequest).ThenInclude(r => r.User)
            .Where(m =>
                m.Status == MatchStatus.PendingConfirmation &&
                (m.SenderRequest.UserId == userId || m.ReceiverRequest.UserId == userId))
            .OrderBy(m => m.ConfirmationDeadline)
            .ToListAsync(ct);

        return matches.Select(m =>
        {
            var isSender = m.SenderRequest.UserId == userId;
            var myRequest = isSender ? m.SenderRequest : m.ReceiverRequest;
            var counterpart = isSender ? m.ReceiverRequest.User : m.SenderRequest.User;
            var myConfirmation = isSender ? m.SenderConfirmed : m.ReceiverConfirmed;

            var hasFullName = !string.IsNullOrEmpty(counterpart.FirstName) && !string.IsNullOrEmpty(counterpart.LastName);
            var counterpartName = hasFullName
                ? $"{counterpart.FirstName} {counterpart.LastName![0]}."
                : counterpart.TelegramUsername ?? "Unknown";

            return new PendingConfirmationMatchDto(
                m.Id,
                myRequest.Id,
                myRequest.Amount,
                myRequest.Currency.ToString(),
                m.Price,
                m.PriceSetAt!.Value,
                m.ConfirmationDeadline!.Value,
                counterpartName,
                counterpart.IsTrusted,
                myConfirmation
            );
        }).ToList();
    }
}
