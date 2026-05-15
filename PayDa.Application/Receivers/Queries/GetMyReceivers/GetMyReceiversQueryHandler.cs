using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Receivers.Queries.GetMyReceivers;

public class GetMyReceiversQueryHandler : IRequestHandler<GetMyReceiversQuery, List<ReceiverDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyReceiversQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ReceiverDto>> Handle(GetMyReceiversQuery request, CancellationToken ct)
    {
        return await _context.Receivers
            .Where(r => r.UserId == _currentUser.UserId)
            .Select(r => new ReceiverDto(r.Id, r.FirstName, r.LastName, r.NationalId, r.MobileNumber, r.IBAN))
            .ToListAsync(ct);
    }
}
