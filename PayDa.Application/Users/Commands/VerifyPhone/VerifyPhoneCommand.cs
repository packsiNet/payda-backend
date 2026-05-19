using MediatR;

namespace PayDa.Application.Users.Commands.VerifyPhone;

public record VerifyPhoneCommand(string PhoneNumber) : IRequest;
