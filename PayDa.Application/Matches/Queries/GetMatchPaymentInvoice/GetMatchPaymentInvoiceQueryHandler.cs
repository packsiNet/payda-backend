using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Matches.Queries.GetMatchPaymentInvoice;

public class GetMatchPaymentInvoiceQueryHandler : IRequestHandler<GetMatchPaymentInvoiceQuery, MatchPaymentInvoiceDto>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMatchPaymentInvoiceQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<MatchPaymentInvoiceDto> Handle(GetMatchPaymentInvoiceQuery request, CancellationToken ct)
    {
        var match = await _context.Matches
            .Include(m => m.SenderRequest).ThenInclude(r => r.Receiver)
            .Include(m => m.SenderRequest).ThenInclude(r => r.User).ThenInclude(u => u.Tier)
            .Include(m => m.ReceiverRequest).ThenInclude(r => r.User).ThenInclude(u => u.Tier)
            .Include(m => m.Transaction)
            .FirstOrDefaultAsync(m => m.Id == request.MatchId, ct)
            ?? throw new NotFoundException("Match not found");

        var userId = _currentUser.UserId;
        var isParticipant = match.SenderRequest.UserId == userId || match.ReceiverRequest.UserId == userId;
        var isAdmin = (await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct))?.Role == UserRole.Admin;

        if (!isParticipant && !isAdmin)
            throw new ForbiddenException("Not a participant of this match");

        if (match.Status != MatchStatus.Active)
            throw new BadRequestException("Match is not confirmed by both parties");

        if (match.Transaction is null)
            throw new BadRequestException("Transaction not created yet");

        var receiver = match.SenderRequest.Receiver
            ?? throw new BadRequestException("Sender request has no receiver info");

        // Commission for EUR receiver (toman_sender) — uses ReceiverCommissionPercent
        var receiverTierId = match.ReceiverRequest.User.TierId;
        var receiverCommission = await _context.TierCommissions
            .FirstOrDefaultAsync(c => c.TierId == receiverTierId, ct);
        var receiverFeePercent = receiverCommission?.ReceiverCommissionPercent ?? 0m;

        // Commission for EUR sender (toman_receiver) — uses SenderCommissionPercent
        var senderTierId = match.SenderRequest.User.TierId;
        var senderCommission = await _context.TierCommissions
            .FirstOrDefaultAsync(c => c.TierId == senderTierId, ct);
        var senderFeePercent = senderCommission?.SenderCommissionPercent ?? 0m;

        var eurAmount = match.SenderRequest.Amount;
        var baseToman = match.Price * eurAmount;

        var tomanSenderAmount = baseToman * (1 + receiverFeePercent / 100);
        var tomanReceiverAmount = baseToman * (1 - senderFeePercent / 100);

        var expireAt = match.Transaction.CreatedAt.AddHours(24);

        return new MatchPaymentInvoiceDto(
            Amount: Math.Round(baseToman, 0),
            Sender: new TomanSenderInvoiceDto(
                Name: match.ReceiverRequest.TomanPayerFullName ?? string.Empty,
                Phone: match.ReceiverRequest.TomanPayerMobileNumber ?? string.Empty,
                Fee: receiverFeePercent,
                Amount: Math.Round(tomanSenderAmount, 0)
            ),
            Receiver: new TomanReceiverInvoiceDto(
                Iban: receiver.IBAN,
                IbanOwnerName: $"{receiver.FirstName} {receiver.LastName}",
                Fee: senderFeePercent,
                Amount: Math.Round(tomanReceiverAmount, 0)
            ),
            InvoiceNumber: match.Transaction.ReferenceCode ?? string.Empty,
            ExpireAt: expireAt
        );
    }
}
