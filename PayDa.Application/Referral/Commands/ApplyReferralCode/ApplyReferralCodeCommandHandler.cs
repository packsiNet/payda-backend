using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Referral.Commands.ApplyReferralCode;

public class ApplyReferralCodeCommandHandler : IRequestHandler<ApplyReferralCodeCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ApplyReferralCodeCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(ApplyReferralCodeCommand request, CancellationToken ct)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, ct)
            ?? throw new NotFoundException("User not found.");

        if (user.ReferredById is not null)
            throw new BadRequestException("Referral code already applied.");

        var referrer = await _context.Users
            .FirstOrDefaultAsync(u => u.ReferralCode == request.ReferralCode.ToUpper(), ct)
            ?? throw new NotFoundException("Invalid referral code.");

        user.ApplyReferral(referrer.Id);
        await _context.SaveChangesAsync(ct);
    }
}
