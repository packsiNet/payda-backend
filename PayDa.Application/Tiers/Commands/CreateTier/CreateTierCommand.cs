using MediatR;

namespace PayDa.Application.Tiers.Commands.CreateTier;

public record CreateTierCommand(
    string Name,
    int Order,
    int MaxActiveRequests,
    decimal MaxAmountPerRequest,
    int RequiredCompletedTransactions
) : IRequest<Guid>;
