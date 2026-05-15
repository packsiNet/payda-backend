using MediatR;

namespace PayDa.Application.Receivers.Commands.CreateReceiver;

public record CreateReceiverCommand(
    string FirstName,
    string LastName,
    string NationalId,
    string MobileNumber,
    string IBAN
) : IRequest<Guid>;
