using MediatR;

namespace PayDa.Application.Users.Commands.SubmitKyc;

public record SubmitKycCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string DateOfBirth,
    Stream SelfieImage,
    string SelfieFileName,
    Stream DocumentImage,
    string DocumentFileName
) : IRequest;
