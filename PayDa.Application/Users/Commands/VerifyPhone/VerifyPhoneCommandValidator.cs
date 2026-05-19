using FluentValidation;

namespace PayDa.Application.Users.Commands.VerifyPhone;

public class VerifyPhoneCommandValidator : AbstractValidator<VerifyPhoneCommand>
{
    public VerifyPhoneCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+[1-9]\d{6,14}$")
            .WithMessage("Phone number must be in international format: +CCXXXXXXXX");
    }
}
