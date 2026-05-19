using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Users.Commands.VerifyPhone;

public class VerifyPhoneCommandHandler : IRequestHandler<VerifyPhoneCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public VerifyPhoneCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(VerifyPhoneCommand cmd, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, ct)
            ?? throw new NotFoundException("User not found");

        user.SetPhoneNumber(VerifyPhoneCommandValidator.Normalize(cmd.PhoneNumber));
        await _context.SaveChangesAsync(ct);
    }
}
