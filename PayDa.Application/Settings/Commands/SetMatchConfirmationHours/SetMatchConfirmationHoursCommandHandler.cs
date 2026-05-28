using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Settings.Commands.SetMatchConfirmationHours;

public class SetMatchConfirmationHoursCommandHandler : IRequestHandler<SetMatchConfirmationHoursCommand>
{
    private readonly IAppDbContext _context;

    public SetMatchConfirmationHoursCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(SetMatchConfirmationHoursCommand cmd, CancellationToken ct)
    {
        if (cmd.Hours < 1 || cmd.Hours > 168)
            throw new BadRequestException("Hours must be between 1 and 168");

        var config = await _context.SystemConfigs.FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("System config not found");

        config.SetMatchConfirmationHours(cmd.Hours);
        await _context.SaveChangesAsync(ct);
    }
}
