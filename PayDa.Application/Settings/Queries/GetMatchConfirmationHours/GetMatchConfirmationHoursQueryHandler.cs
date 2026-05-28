using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Settings.Queries.GetMatchConfirmationHours;

public class GetMatchConfirmationHoursQueryHandler : IRequestHandler<GetMatchConfirmationHoursQuery, int>
{
    private readonly IAppDbContext _context;

    public GetMatchConfirmationHoursQueryHandler(IAppDbContext context) => _context = context;

    public async Task<int> Handle(GetMatchConfirmationHoursQuery request, CancellationToken ct)
    {
        var config = await _context.SystemConfigs.FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("System config not found");
        return config.MatchConfirmationHours;
    }
}
