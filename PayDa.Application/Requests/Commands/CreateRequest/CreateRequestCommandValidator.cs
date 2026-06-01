using FluentValidation;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Commands.CreateRequest;

public class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
{
    public CreateRequestCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.PaymentMethods).NotEmpty();

        // Send: either ReceiverId or NewReceiver required
        RuleFor(x => x.ReceiverId)
            .NotEmpty()
            .When(x => x.Type == RequestType.Send && x.NewReceiver is null);

        RuleFor(x => x.NewReceiver)
            .NotNull()
            .When(x => x.Type == RequestType.Send && x.ReceiverId is null)
            .ChildRules(nr =>
            {
                nr.RuleFor(r => r.FirstName).NotEmpty().MaximumLength(100);
                nr.RuleFor(r => r.LastName).NotEmpty().MaximumLength(100);
                nr.RuleFor(r => r.NationalId).NotEmpty().Length(10);
                nr.RuleFor(r => r.MobileNumber).NotEmpty();
                nr.RuleFor(r => r.IBAN).NotEmpty().MaximumLength(34);
            })
            .When(x => x.Type == RequestType.Send && x.NewReceiver is not null);

        // Receive: TomanPayer required
        RuleFor(x => x.TomanPayer)
            .NotNull()
            .When(x => x.Type == RequestType.Receive)
            .ChildRules(tp =>
            {
                tp.RuleFor(t => t.FullName).NotEmpty().MaximumLength(200);
                tp.RuleFor(t => t.MobileNumber).NotEmpty().MaximumLength(20);
            })
            .When(x => x.Type == RequestType.Receive && x.TomanPayer is not null);

        // Receive: at least one foreign account required
        RuleFor(x => x.ForeignAccounts)
            .NotEmpty()
            .When(x => x.Type == RequestType.Receive);

        RuleForEach(x => x.ForeignAccounts)
            .ChildRules(fa =>
            {
                fa.RuleFor(f => f.FullName).NotEmpty().MaximumLength(200);

                fa.RuleFor(f => f.Username)
                    .NotEmpty()
                    .When(f => f.Method == PaymentMethod.Revolut);

                fa.RuleFor(f => f.EmailOrPhone)
                    .NotEmpty()
                    .When(f => f.Method == PaymentMethod.Zelle);

                fa.RuleFor(f => f.Email)
                    .NotEmpty()
                    .When(f => f.Method == PaymentMethod.PayPal);

                fa.RuleFor(f => f.Iban)
                    .NotEmpty()
                    .When(f => f.Method == PaymentMethod.SEPA);

                fa.RuleFor(f => f.Bic)
                    .NotEmpty()
                    .When(f => f.Method == PaymentMethod.SEPA);

                fa.RuleFor(f => f.BankName)
                    .NotEmpty()
                    .When(f => f.Method == PaymentMethod.SEPA || f.Method == PaymentMethod.Wire);

                fa.RuleFor(f => f.AccountNum)
                    .NotEmpty()
                    .When(f => f.Method == PaymentMethod.Wire);

                fa.RuleFor(f => f.Swift)
                    .NotEmpty()
                    .When(f => f.Method == PaymentMethod.Wire);
            })
            .When(x => x.Type == RequestType.Receive && x.ForeignAccounts != null);
    }
}
