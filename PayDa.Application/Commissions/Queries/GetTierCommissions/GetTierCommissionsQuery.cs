using MediatR;

namespace PayDa.Application.Commissions.Queries.GetTierCommissions;

public record GetTierCommissionsQuery : IRequest<List<TierCommissionDto>>;

public record TierCommissionDto(
    Guid TierCommissionId,
    Guid TierId,
    string TierName,
    int TierOrder,
    decimal SenderCommissionPercent,
    decimal ReceiverCommissionPercent
);
