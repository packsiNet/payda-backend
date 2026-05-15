using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Users.Commands.SetUserRole;

public record SetUserRoleCommand(Guid UserId, UserRole Role) : IRequest;
