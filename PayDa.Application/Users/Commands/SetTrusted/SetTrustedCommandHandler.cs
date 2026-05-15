using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Users.Commands.SetTrusted;

public class SetTrustedCommandHandler : IRequestHandler<SetTrustedCommand>
{
    private readonly IAppDbContext _context;

    public SetTrustedCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(SetTrustedCommand cmd, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == cmd.UserId, ct)
            ?? throw new NotFoundException("User not found");

        user.SetTrusted(cmd.IsTrusted);
        await _context.SaveChangesAsync(ct);
    }
}
