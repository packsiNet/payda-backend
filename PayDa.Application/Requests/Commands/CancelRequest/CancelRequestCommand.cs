using MediatR;

namespace PayDa.Application.Requests.Commands.CancelRequest;

public record CancelRequestCommand(Guid Id) : IRequest;
