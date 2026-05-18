using MediatR;

namespace PayDa.Application.Matches.Commands.CreateUserMatch;

public record CreateUserMatchCommand(Guid RequestId) : IRequest<CreateUserMatchResult>;

public record CreateUserMatchResult(Guid MatchId, string Message);
