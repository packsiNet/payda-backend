using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Users.Queries.GetPendingKycUsers;

public class GetPendingKycUsersQueryHandler : IRequestHandler<GetPendingKycUsersQuery, List<PendingKycUserDto>>
{
    private readonly IAppDbContext _context;

    public GetPendingKycUsersQueryHandler(IAppDbContext context) => _context = context;

    public async Task<List<PendingKycUserDto>> Handle(GetPendingKycUsersQuery request, CancellationToken ct)
    {
        return await _context.Users
            .Where(u => u.KycStatus == KycStatus.Pending)
            .OrderBy(u => u.KycSubmittedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new PendingKycUserDto(
                u.Id, u.TelegramId, u.TelegramUsername,
                u.FirstName, u.LastName, u.DateOfBirth,
                u.PhoneNumber, u.SelfieImageUrl, u.DocumentImageUrl,
                u.KycSubmittedAt))
            .ToListAsync(ct);
    }
}
