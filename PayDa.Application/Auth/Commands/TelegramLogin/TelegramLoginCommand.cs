using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Auth.Commands.TelegramLogin;

public record TelegramLoginCommand(string InitData) : IRequest<TelegramLoginResult>;
public record TelegramLoginResult(string Token, bool IsNewUser, KycStatus KycStatus);
