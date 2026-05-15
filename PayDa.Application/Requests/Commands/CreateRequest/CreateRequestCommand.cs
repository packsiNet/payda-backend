using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Commands.CreateRequest;

public record CreateRequestCommand(
    RequestType Type,
    Currency Currency,
    decimal Amount,
    RateType RateType,
    decimal? CustomRate,
    List<PaymentMethod> PaymentMethods,
    Guid ReceiverId
) : IRequest<Guid>;
