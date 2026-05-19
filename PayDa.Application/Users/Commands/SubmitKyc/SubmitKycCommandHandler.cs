using MediatR;
using Microsoft.EntityFrameworkCore;
using PayDa.Application.Common.Exceptions;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Application.Users.Commands.SubmitKyc;

public class SubmitKycCommandHandler : IRequestHandler<SubmitKycCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IStorageService _storage;

    public SubmitKycCommandHandler(IAppDbContext context, ICurrentUserService currentUser, IStorageService storage)
    {
        _context = context;
        _currentUser = currentUser;
        _storage = storage;
    }

    public async Task Handle(SubmitKycCommand cmd, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, ct)
            ?? throw new NotFoundException("User not found");

        if (string.IsNullOrEmpty(user.PhoneNumber))
            throw new BadRequestException("Phone number must be verified before submitting KYC.");

        var selfieUrl = await _storage.UploadAsync(cmd.SelfieImage, cmd.SelfieFileName, "kyc/selfies", ct);
        var documentUrl = await _storage.UploadAsync(cmd.DocumentImage, cmd.DocumentFileName, "kyc/documents", ct);

        user.SubmitKyc(cmd.FirstName, cmd.LastName, cmd.DateOfBirth, selfieUrl, documentUrl);
        await _context.SaveChangesAsync(ct);
    }
}
