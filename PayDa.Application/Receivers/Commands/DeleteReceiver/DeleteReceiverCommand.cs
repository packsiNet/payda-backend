using MediatR;

namespace PayDa.Application.Receivers.Commands.DeleteReceiver;

public record DeleteReceiverCommand(Guid Id) : IRequest;
