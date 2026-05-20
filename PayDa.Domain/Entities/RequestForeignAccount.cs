using PayDa.Domain.Common;
using PayDa.Domain.Enums;

namespace PayDa.Domain.Entities;

public class RequestForeignAccount : BaseEntity
{
    public Guid RequestId { get; private set; }
    public Request Request { get; private set; } = default!;

    public PaymentMethod Method { get; private set; }
    public string FullName { get; private set; } = default!;

    // Revolut
    public string? Username { get; private set; }
    public string? Email { get; private set; }

    // Zelle
    public string? EmailOrPhone { get; private set; }

    // SEPA
    public string? Iban { get; private set; }
    public string? Bic { get; private set; }
    public string? BankName { get; private set; }

    // Wire
    public string? AccountNum { get; private set; }
    public string? Swift { get; private set; }
    public string? BankAddress { get; private set; }

    private RequestForeignAccount() { }

    public static RequestForeignAccount Create(
        Guid requestId,
        PaymentMethod method,
        string fullName,
        string? username = null,
        string? email = null,
        string? emailOrPhone = null,
        string? iban = null,
        string? bic = null,
        string? bankName = null,
        string? accountNum = null,
        string? swift = null,
        string? bankAddress = null) => new()
    {
        RequestId = requestId,
        Method = method,
        FullName = fullName,
        Username = username,
        Email = email,
        EmailOrPhone = emailOrPhone,
        Iban = iban,
        Bic = bic,
        BankName = bankName,
        AccountNum = accountNum,
        Swift = swift,
        BankAddress = bankAddress
    };
}
