using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserSummaryDto>>
{
    private readonly IAppDbContext _context;

    public GetAllUsersQueryHandler(IAppDbContext context) => _context = context;

    public async Task<List<UserSummaryDto>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        return await _context.Users
            .Include(u => u.Tier)
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserSummaryDto(
                u.Id, u.TelegramId, u.TelegramUsername,
                u.FirstName, u.LastName, u.KycStatus,
                u.Role, u.IsTrusted, u.Tier.Name, u.CreatedAt,
                u.SelfieImageUrl, u.DocumentImageUrl))
            .ToListAsync(ct);
    }
}
