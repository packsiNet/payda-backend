using MediatR;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Entities;

namespace PayDa.Application.Tiers.Commands.CreateTier;

public class CreateTierCommandHandler : IRequestHandler<CreateTierCommand, Guid>
{
    private readonly IAppDbContext _context;

    public CreateTierCommandHandler(IAppDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateTierCommand cmd, CancellationToken ct)
    {
        var tier = Tier.Create(cmd.Name, cmd.Order, cmd.MaxActiveRequests,
            cmd.MaxAmountPerRequest, cmd.RequiredCompletedTransactions);

        _context.Tiers.Add(tier);
        await _context.SaveChangesAsync(ct);
        return tier.Id;
    }
}
