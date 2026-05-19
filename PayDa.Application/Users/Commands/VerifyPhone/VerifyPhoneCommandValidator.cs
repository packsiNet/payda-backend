using System.Text.RegularExpressions;
using FluentValidation;

namespace PayDa.Application.Users.Commands.VerifyPhone;

public class VerifyPhoneCommandValidator : AbstractValidator<VerifyPhoneCommand>
{
    private static readonly Regex E164Regex = new(@"^\+[1-9]\d{6,14}$", RegexOptions.Compiled);

    public VerifyPhoneCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Must(phone => E164Regex.IsMatch(Normalize(phone ?? "")))
            .WithMessage("Phone number must be in international format: +CCXXXXXXXX (e.g. +989123456789)");
    }

    public static string Normalize(string phone)
    {
        var cleaned = phone.Trim().Replace(" ", "").Replace("-", "");
        if (cleaned.StartsWith("00"))
            cleaned = "+" + cleaned[2..];
        else if (!cleaned.StartsWith("+"))
            cleaned = "+" + cleaned;
        return cleaned;
    }
}
