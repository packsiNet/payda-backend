using MediatR;

namespace PayDa.Application.Matches.Commands.MatchRequests;

public record MatchRequestsCommand(Guid SenderRequestId, Guid ReceiverRequestId, bool IsAgentInvolved = false) : IRequest<Guid>;
