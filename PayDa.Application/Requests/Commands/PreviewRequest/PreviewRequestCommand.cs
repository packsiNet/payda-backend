using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Commands.PreviewRequest;

public record PreviewRequestCommand(
    RequestType Type,
    Currency Currency,
    decimal Amount,
    PricePreference PricePreference
) : IRequest<PreviewRequestResult>;

public record PreviewRequestResult(
    decimal Amount,
    Currency Currency,
    PricePreference PricePreference
);
