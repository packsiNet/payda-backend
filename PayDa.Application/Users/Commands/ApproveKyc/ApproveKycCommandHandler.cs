using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Users.Commands.ApproveKyc;

public class ApproveKycCommandHandler : IRequestHandler<ApproveKycCommand>
{
    private readonly IAppDbContext _context;

    public ApproveKycCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(ApproveKycCommand cmd, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == cmd.UserId, ct)
            ?? throw new NotFoundException("User not found");

        user.ApproveKyc();
        await _context.SaveChangesAsync(ct);
    }
}
