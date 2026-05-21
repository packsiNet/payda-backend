using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Auth.Commands.AdminWidgetLogin;

public class AdminWidgetLoginCommandHandler : IRequestHandler<AdminWidgetLoginCommand, AdminWidgetLoginResult>
{
    private readonly IAppDbContext _context;
    private readonly ITelegramAuthService _telegramAuth;
    private readonly IJwtService _jwtService;

    public AdminWidgetLoginCommandHandler(
        IAppDbContext context,
        ITelegramAuthService telegramAuth,
        IJwtService jwtService)
    {
        _context = context;
        _telegramAuth = telegramAuth;
        _jwtService = jwtService;
    }

    public async Task<AdminWidgetLoginResult> Handle(AdminWidgetLoginCommand request, CancellationToken ct)
    {
        if (!_telegramAuth.ValidateWidgetData(request.WidgetData))
            throw new UnauthorizedException("Invalid Telegram widget data");

        var telegramUser = _telegramAuth.ParseWidgetData(request.WidgetData);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.TelegramId == telegramUser.Id, ct)
            ?? throw new ForbiddenException("Access denied: user not found");

        if (user.Role != UserRole.Admin)
            throw new ForbiddenException("Access denied: admin role required");

        var token = _jwtService.GenerateToken(user);
        return new AdminWidgetLoginResult(token);
    }
}
