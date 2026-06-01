using MediatR;

namespace PayDa.Application.Commissions.Commands.SetTierCommission;

public record SetTierCommissionCommand(
    Guid TierId,
    decimal SenderCommissionPercent,
    decimal ReceiverCommissionPercent
) : IRequest;
