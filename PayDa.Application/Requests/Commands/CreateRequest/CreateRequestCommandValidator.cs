using FluentValidation;
using PayDa.Domain.Enums;

namespace PayDa.Application.Requests.Commands.CreateRequest;

public class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
{
    public CreateRequestCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.PaymentMethods).NotEmpty();
        RuleFor(x => x.ReceiverId).NotEmpty();
        RuleFor(x => x.CustomRate)
            .GreaterThan(0)
            .When(x => x.RateType == RateType.Custom);
    }
}
