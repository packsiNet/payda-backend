using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Matches.Queries.GetMyMatches;

public class GetMyMatchesQueryHandler : IRequestHandler<GetMyMatchesQuery, List<MyMatchDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyMatchesQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<MyMatchDto>> Handle(GetMyMatchesQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var matches = await _context.Matches
            .Include(m => m.SenderRequest).ThenInclude(r => r.User).ThenInclude(u => u.Tier)
            .Include(m => m.SenderRequest).ThenInclude(r => r.ForeignAccounts)
            .Include(m => m.ReceiverRequest).ThenInclude(r => r.User).ThenInclude(u => u.Tier)
            .Include(m => m.ReceiverRequest).ThenInclude(r => r.ForeignAccounts)
            .Include(m => m.Transaction)
            .Where(m =>
                m.SenderRequest.UserId == userId ||
                m.ReceiverRequest.UserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(ct);

        return matches
            .Select(m =>
            {
                var isSender = m.SenderRequest.UserId == userId;
                var myRequest = isSender ? m.SenderRequest : m.ReceiverRequest;
                var counterpartRequest = isSender ? m.ReceiverRequest : m.SenderRequest;
                var counterpart = counterpartRequest.User;

                var hasFullName = !string.IsNullOrEmpty(counterpart.FirstName) && !string.IsNullOrEmpty(counterpart.LastName);
                var counterpartName = hasFullName
                    ? $"{counterpart.FirstName} {counterpart.LastName![0]}."
                    : counterpart.TelegramUsername ?? "Unknown";

                var myConfirmed = isSender ? m.SenderConfirmed : m.ReceiverConfirmed;
                var counterpartConfirmed = isSender ? m.ReceiverConfirmed : m.SenderConfirmed;

                var flowStatus = ResolveFlowStatus(myConfirmed, counterpartConfirmed, m.Transaction?.Status);

                return new MyMatchDto(
                    m.Id,
                    myRequest.Id,
                    myRequest.Type,
                    myRequest.Currency,
                    myRequest.Amount,
                    myRequest.PricePreference,
                    m.Price,
                    myRequest.ExpiresAt,
                    myRequest.CreatedAt,
                    m.CreatedAt,
                    counterpartName,
                    counterpart.Tier?.Order ?? 0,
                    counterpart.Tier?.Name ?? string.Empty,
                    counterpart.IsTrusted,
                    counterpartRequest.ForeignAccounts?.Select(a => new CounterpartPaymentMethodDto(
                        a.Method.ToString(),
                        a.FullName,
                        a.Username,
                        a.Email,
                        a.EmailOrPhone,
                        a.Iban,
                        a.Bic,
                        a.BankName,
                        a.AccountNum,
                        a.Swift,
                        a.BankAddress
                    )).ToList() ?? [],
                    m.Transaction?.Id,
                    flowStatus
                );
            })
            .ToList();
    }

    private static MatchFlowStatus ResolveFlowStatus(bool myConfirmed, bool counterpartConfirmed, TransactionStatus? txStatus)
    {
        if (txStatus != null)
        {
            return txStatus switch
            {
                TransactionStatus.WaitingForTomanPayment => MatchFlowStatus.WaitingForTomanPayment,
                TransactionStatus.TomanPaymentDeclared => MatchFlowStatus.TomanPaymentDeclared,
                TransactionStatus.TomanConfirmed => MatchFlowStatus.WaitingForForeignTransfer,
                TransactionStatus.ForeignReceiptUploaded => MatchFlowStatus.ForeignReceiptUploaded,
                TransactionStatus.ForeignReceiptConfirmed => MatchFlowStatus.Completed,
                TransactionStatus.Completed => MatchFlowStatus.Completed,
                TransactionStatus.Disputed => MatchFlowStatus.Disputed,
                _ => MatchFlowStatus.WaitingForTomanPayment
            };
        }

        if (myConfirmed && counterpartConfirmed) return MatchFlowStatus.WaitingForTomanPayment;
        if (myConfirmed) return MatchFlowStatus.WaitingForCounterpartConfirmation;
        if (counterpartConfirmed) return MatchFlowStatus.WaitingForMyConfirmation;
        return MatchFlowStatus.WaitingForBothConfirmations;
    }
}
