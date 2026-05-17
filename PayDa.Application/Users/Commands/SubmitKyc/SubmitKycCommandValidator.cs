using FluentValidation;

namespace PayDa.Application.Users.Commands.SubmitKyc;

public class SubmitKycCommandValidator : AbstractValidator<SubmitKycCommand>
{
    public SubmitKycCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+[1-9]\d{6,14}$")
            .WithMessage("Phone number must be in international format: +CCXXXXXXXX");
        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .Matches(@"^\d{4}-\d{2}-\d{2}$")
            .WithMessage("DateOfBirth must be in format YYYY-MM-DD");
    }
}
