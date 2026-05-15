using MediatR;

namespace PayDa.Application.Users.Commands.ApproveKyc;

public record ApproveKycCommand(Guid UserId) : IRequest;
