using MediatR;

namespace PayDa.Application.Matches.Commands.ConfirmMatch;

public record ConfirmMatchCommand(Guid MatchId) : IRequest;
