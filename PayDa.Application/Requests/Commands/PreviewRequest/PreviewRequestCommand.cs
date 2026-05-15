using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Commands.PreviewRequest;

public record PreviewRequestCommand(
    RequestType Type,
    Currency Currency,
    decimal Amount,
    RateType RateType,
    decimal? CustomRate
) : IRequest<PreviewRequestResult>;

public record PreviewRequestResult(
    decimal Amount,
    decimal RateValue,
    decimal CommissionPercent,
    decimal CommissionAmount,
    decimal TotalAmount
);
