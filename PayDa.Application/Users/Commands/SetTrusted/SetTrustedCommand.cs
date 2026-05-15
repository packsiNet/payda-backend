using MediatR;

namespace PayDa.Application.Users.Commands.SetTrusted;

public record SetTrustedCommand(Guid UserId, bool IsTrusted) : IRequest;
