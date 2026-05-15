using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Receivers.Commands.DeleteReceiver;

public class DeleteReceiverCommandHandler : IRequestHandler<DeleteReceiverCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteReceiverCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteReceiverCommand cmd, CancellationToken ct)
    {
        var receiver = await _context.Receivers
            .FirstOrDefaultAsync(r => r.Id == cmd.Id && r.UserId == _currentUser.UserId, ct)
            ?? throw new NotFoundException("Receiver not found");

        _context.Receivers.Remove(receiver);
        await _context.SaveChangesAsync(ct);
    }
}
