using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Commands.CreateRequest;

public record CreateRequestCommand(
    RequestType Type,
    Currency Currency,
    decimal Amount,
    PricePreference PricePreference,
    List<PaymentMethod> PaymentMethods,
    // Send only — either ReceiverId (existing) or NewReceiver (inline create)
    Guid? ReceiverId,
    NewReceiverDto? NewReceiver,
    // Receive only
    List<ForeignAccountDto>? ForeignAccounts
) : IRequest<Guid>;

public record NewReceiverDto(
    string FirstName,
    string LastName,
    string NationalId,
    string MobileNumber,
    string IBAN
);

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
