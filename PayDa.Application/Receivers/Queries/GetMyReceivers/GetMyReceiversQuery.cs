using MediatR;

namespace PayDa.Application.Receivers.Queries.GetMyReceivers;

public record GetMyReceiversQuery : IRequest<List<ReceiverDto>>;

public record ReceiverDto(Guid Id, string FirstName, string LastName, string NationalId, string MobileNumber, string IBAN);
