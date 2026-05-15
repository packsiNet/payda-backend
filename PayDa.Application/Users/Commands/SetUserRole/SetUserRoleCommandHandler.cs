using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Users.Commands.SetUserRole;

public class SetUserRoleCommandHandler : IRequestHandler<SetUserRoleCommand>
{
    private readonly IAppDbContext _context;

    public SetUserRoleCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(SetUserRoleCommand cmd, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == cmd.UserId, ct)
            ?? throw new NotFoundException("User not found");

        user.SetRole(cmd.Role);
        await _context.SaveChangesAsync(ct);
    }
}
