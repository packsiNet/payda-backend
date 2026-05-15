using MediatR;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Entities;

namespace PayDa.Application.Receivers.Commands.CreateReceiver;

public class CreateReceiverCommandHandler : IRequestHandler<CreateReceiverCommand, Guid>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateReceiverCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateReceiverCommand cmd, CancellationToken ct)
    {
        var receiver = Receiver.Create(_currentUser.UserId, cmd.FirstName, cmd.LastName,
            cmd.NationalId, cmd.MobileNumber, cmd.IBAN);

        _context.Receivers.Add(receiver);
        await _context.SaveChangesAsync(ct);
        return receiver.Id;
    }
}
