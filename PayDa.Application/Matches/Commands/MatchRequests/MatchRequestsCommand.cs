using MediatR;

namespace PayDa.Application.Matches.Commands.MatchRequests;

public record MatchRequestsCommand(
    Guid SenderRequestId,
    Guid ReceiverRequestId,
    decimal Price,
    bool IsAgentInvolved = false
) : IRequest<Guid>;
