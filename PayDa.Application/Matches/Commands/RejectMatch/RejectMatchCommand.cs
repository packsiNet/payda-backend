using MediatR;

namespace PayDa.Application.Matches.Commands.RejectMatch;

public record RejectMatchCommand(Guid MatchId) : IRequest;
