using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Entities;

namespace PayDa.Application.Commissions.Commands.SetTierCommission;

public class SetTierCommissionCommandHandler : IRequestHandler<SetTierCommissionCommand>
{
    private readonly IAppDbContext _context;

    public SetTierCommissionCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(SetTierCommissionCommand cmd, CancellationToken ct)
    {
        var tierExists = await _context.Tiers.AnyAsync(t => t.Id == cmd.TierId, ct);
        if (!tierExists)
            throw new NotFoundException($"Tier '{cmd.TierId}' not found.");

        var commission = await _context.TierCommissions
            .FirstOrDefaultAsync(c => c.TierId == cmd.TierId, ct);

        if (commission is null)
        {
            commission = TierCommission.Create(cmd.TierId, cmd.SenderCommissionPercent, cmd.ReceiverCommissionPercent);
            _context.TierCommissions.Add(commission);
        }
        else
        {
            commission.Update(cmd.SenderCommissionPercent, cmd.ReceiverCommissionPercent);
        }

        await _context.SaveChangesAsync(ct);
    }
}
