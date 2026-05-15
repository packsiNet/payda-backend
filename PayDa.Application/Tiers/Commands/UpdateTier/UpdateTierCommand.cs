using MediatR;

namespace PayDa.Application.Tiers.Commands.UpdateTier;

public record UpdateTierCommand(
    Guid Id,
    string Name,
    int MaxActiveRequests,
    decimal MaxAmountPerRequest,
    int RequiredCompletedTransactions
) : IRequest;
