using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Users.Commands.RejectKyc;

public class RejectKycCommandHandler : IRequestHandler<RejectKycCommand>
{
    private readonly IAppDbContext _context;

    public RejectKycCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(RejectKycCommand cmd, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == cmd.UserId, ct)
            ?? throw new NotFoundException("User not found");

        user.RejectKyc();
        await _context.SaveChangesAsync(ct);
    }
}
