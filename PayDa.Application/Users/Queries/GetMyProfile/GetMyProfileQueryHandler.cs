using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Users.Queries.GetMyProfile;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, UserProfileDto>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyProfileQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<UserProfileDto> Handle(GetMyProfileQuery request, CancellationToken ct)
    {
        var user = await _context.Users
            .Include(u => u.Tier)
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, ct)
            ?? throw new NotFoundException("User not found");

        return new UserProfileDto(
            user.Id, user.TelegramId, user.TelegramUsername,
            user.FirstName, user.LastName, user.KycStatus,
            user.Role, user.IsTrusted, user.Tier.Name,
            user.Tier.Order, user.CompletedTransactionsCount,
            user.PhoneNumber != null,
            user.SelfieImageUrl,
            user.DocumentImageUrl);
    }
}
