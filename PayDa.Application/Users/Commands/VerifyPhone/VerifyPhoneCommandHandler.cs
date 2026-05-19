using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Users.Commands.VerifyPhone;

public class VerifyPhoneCommandHandler : IRequestHandler<VerifyPhoneCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ITelegramAuthService _telegramAuth;

    public VerifyPhoneCommandHandler(IAppDbContext context, ICurrentUserService currentUser, ITelegramAuthService telegramAuth)
    {
        _context = context;
        _currentUser = currentUser;
        _telegramAuth = telegramAuth;
    }

    public async Task Handle(VerifyPhoneCommand cmd, CancellationToken ct)
    {
        if (!_telegramAuth.ValidateContactResponse(cmd.TelegramResponse))
            throw new UnauthorizedException("Invalid Telegram contact response");

        var contact = _telegramAuth.ParseContactResponse(cmd.TelegramResponse);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, ct)
            ?? throw new NotFoundException("User not found");

        user.SetPhoneNumber(contact.PhoneNumber);
        await _context.SaveChangesAsync(ct);
    }
}
