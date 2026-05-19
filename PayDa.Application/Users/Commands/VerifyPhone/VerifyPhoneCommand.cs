using MediatR;

namespace PayDa.Application.Users.Commands.VerifyPhone;

public record VerifyPhoneCommand(string TelegramResponse) : IRequest;
