using FluentValidation;

namespace PayDa.Application.Users.Commands.VerifyPhone;

public class VerifyPhoneCommandValidator : AbstractValidator<VerifyPhoneCommand>
{
    public VerifyPhoneCommandValidator()
    {
        RuleFor(x => x.TelegramResponse)
            .NotEmpty()
            .WithMessage("Telegram contact response is required")
            .Must(r => r != null && r.Contains("contact=") && r.Contains("hash="))
            .WithMessage("Invalid Telegram contact response format");
    }
}
