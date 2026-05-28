using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Commands.CreateRequest;

public record CreateRequestCommand(
    RequestType Type,
    Currency Currency,
    decimal Amount,
    PricePreference PricePreference,
    List<PaymentMethod> PaymentMethods,
    // Send only
    Guid? ReceiverId,
    // Receive only
    List<ForeignAccountDto>? ForeignAccounts
) : IRequest<Guid>;

public record ForeignAccountDto(
    PaymentMethod Method,
    string FullName,
    // Revolut
    string? Username,
    string? Email,
    // Zelle
    string? EmailOrPhone,
    // SEPA
    string? Iban,
    string? Bic,
    string? BankName,
    // Wire
    string? AccountNum,
    string? Swift,
    string? BankAddress
);
