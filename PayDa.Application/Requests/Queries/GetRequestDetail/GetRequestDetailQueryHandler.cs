using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Requests.Queries.GetRequestDetail;

public class GetRequestDetailQueryHandler : IRequestHandler<GetRequestDetailQuery, RequestDetailDto>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetRequestDetailQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<RequestDetailDto> Handle(GetRequestDetailQuery request, CancellationToken ct)
    {
        var r = await _context.Requests
            .Include(x => x.ForeignAccounts)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == _currentUser.UserId, ct)
            ?? throw new NotFoundException("Request not found");

        return new RequestDetailDto(
            r.Id, r.Type, r.Currency, r.Amount, r.PricePreference,
            r.PaymentMethods.Select(p => p.ToString()).ToList(),
            r.Status, r.ExpiresAt, r.CreatedAt,
            r.ReceiverId,
            r.ForeignAccounts.Select(f => new ForeignAccountDetailDto(
                f.Id, f.Method, f.FullName,
                f.Username, f.Email, f.EmailOrPhone,
                f.Iban, f.Bic, f.BankName,
                f.AccountNum, f.Swift, f.BankAddress
            )).ToList()
        );
    }
}
