using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Commands.CancelRequest;

public class CancelRequestCommandHandler : IRequestHandler<CancelRequestCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CancelRequestCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(CancelRequestCommand cmd, CancellationToken ct)
    {
        var request = await _context.Requests
            .FirstOrDefaultAsync(r => r.Id == cmd.Id && r.UserId == _currentUser.UserId, ct)
            ?? throw new NotFoundException("Request not found");

        if (request.Status != RequestStatus.Pending)
            throw new ForbiddenException("Only pending requests can be cancelled");

        request.Cancel();
        await _context.SaveChangesAsync(ct);
    }
}
