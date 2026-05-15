using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Tiers.Commands.UpdateTier;

public class UpdateTierCommandHandler : IRequestHandler<UpdateTierCommand>
{
    private readonly IAppDbContext _context;

    public UpdateTierCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(UpdateTierCommand cmd, CancellationToken ct)
    {
        var tier = await _context.Tiers.FirstOrDefaultAsync(t => t.Id == cmd.Id, ct)
            ?? throw new NotFoundException("Tier not found");

        tier.Update(cmd.Name, cmd.MaxActiveRequests, cmd.MaxAmountPerRequest,
            cmd.RequiredCompletedTransactions);
        await _context.SaveChangesAsync(ct);
    }
}
