using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Matches.Queries.GetMyMatches;

public record GetMyMatchesQuery : IRequest<List<MyMatchDto>>;

public record MyMatchDto(
    Guid MatchId,
    Guid MyRequestId,
    RequestType MyRequestType,
    Currency Currency,
    decimal Amount,
    PricePreference PricePreference,
    decimal MatchPrice,
    DateTime ExpiresAt,
    DateTime RequestDate,
    DateTime MatchDate,
    string CounterpartDisplayName,
    int CounterpartLevel,
    string CounterpartLevelTitle,
    bool CounterpartIsTrusted,
    List<CounterpartPaymentMethodDto> CounterpartPaymentMethods,
    Guid? TransactionId,
    MatchFlowStatus Status
);

public record CounterpartPaymentMethodDto(
    string Method,
    string FullName,
    string? Username,
    string? Email,
    string? EmailOrPhone,
    string? Iban,
    string? Bic,
    string? BankName,
    string? AccountNum,
    string? Swift,
    string? BankAddress
);
