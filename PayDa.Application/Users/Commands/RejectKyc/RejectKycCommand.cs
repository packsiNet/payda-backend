using MediatR;

namespace PayDa.Application.Users.Commands.RejectKyc;

public record RejectKycCommand(Guid UserId) : IRequest;
