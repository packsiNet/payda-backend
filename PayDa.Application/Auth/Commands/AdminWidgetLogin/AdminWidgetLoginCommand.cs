using MediatR;

namespace PayDa.Application.Auth.Commands.AdminWidgetLogin;

public record AdminWidgetLoginCommand(Dictionary<string, string> WidgetData) : IRequest<AdminWidgetLoginResult>;

public record AdminWidgetLoginResult(string Token);
