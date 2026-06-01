using MediatR;

namespace PayDa.Application.Matches.Queries.GetMatchPaymentInvoice;

public record GetMatchPaymentInvoiceQuery(Guid MatchId) : IRequest<MatchPaymentInvoiceDto>;

public record MatchPaymentInvoiceDto(
    TomanSenderInvoiceDto Sender,
    TomanReceiverInvoiceDto Receiver,
    decimal Amount,
    string InvoiceNumber,
    DateTime ExpireAt
);

public record TomanSenderInvoiceDto(
    string Name,
    string Phone,
    decimal Fee,
    decimal Amount
);

public record TomanReceiverInvoiceDto(
    string Iban,
    string IbanOwnerName,
    decimal Fee,
    decimal Amount
);
