using MediatR;

namespace PayDa.Application.Tiers.Queries.GetAllTiers;

public record GetAllTiersQuery : IRequest<List<TierDto>>;

public record TierDto(
    Guid Id,
    string Name,
    int Order,
    int MaxActiveRequests,
    decimal MaxAmountPerRequest,
    int RequiredCompletedTransactions
);
