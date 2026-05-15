using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Queries.GetRequestDetail;

public record GetRequestDetailQuery(Guid Id) : IRequest<RequestDetailDto>;

public record RequestDetailDto(
    Guid Id,
    RequestType Type,
    Currency Currency,
    decimal Amount,
    RateType RateType,
    decimal RateValue,
    decimal CommissionPercent,
    decimal CommissionAmount,
    List<string> PaymentMethods,
    RequestStatus Status,
    DateTime ExpiresAt,
    DateTime CreatedAt,
    Guid ReceiverId
);
