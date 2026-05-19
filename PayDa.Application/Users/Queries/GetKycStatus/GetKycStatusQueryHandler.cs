using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Users.Queries.GetKycStatus;

public class GetKycStatusQueryHandler : IRequestHandler<GetKycStatusQuery, KycStatusDto>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetKycStatusQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<KycStatusDto> Handle(GetKycStatusQuery request, CancellationToken ct)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, ct)
            ?? throw new NotFoundException("User not found");

        var displayName = user.KycStatus switch
        {
            KycStatus.NotSubmitted => "ثبت نشده",
            KycStatus.Pending => "در حال بررسی",
            KycStatus.Approved => "تایید شده",
            KycStatus.Rejected => "رد شده",
            _ => "نامشخص"
        };

        return new KycStatusDto(user.KycStatus, displayName);
    }
}
