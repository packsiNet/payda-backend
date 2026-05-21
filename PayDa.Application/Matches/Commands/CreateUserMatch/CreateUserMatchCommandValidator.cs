using FluentValidation;
using PayDa.Domain.Enums;

namespace PayDa.Application.Matches.Commands.CreateUserMatch;

public class CreateUserMatchCommandValidator : AbstractValidator<CreateUserMatchCommand>
{
    public CreateUserMatchCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty();

        When(x => x.ReceiverInfo != null, () =>
        {
            RuleFor(x => x.ReceiverInfo!.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ReceiverInfo!.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ReceiverInfo!.NationalId).NotEmpty().Length(10);
            RuleFor(x => x.ReceiverInfo!.MobileNumber).NotEmpty().MaximumLength(20);
            RuleFor(x => x.ReceiverInfo!.IBAN).NotEmpty().MaximumLength(34);
        });

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
            .When(x => x.ForeignAccounts != null);
    }
}
