using PayDa.Domain.Common;

namespace PayDa.Domain.Entities;

public class Receiver : BaseEntity
{
    public Guid UserId { get; private set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string NationalId { get; private set; } = default!;
    public string MobileNumber { get; private set; } = default!;
    public string IBAN { get; private set; } = default!;

    private Receiver() { }

    public static Receiver Create(Guid userId, string firstName, string lastName,
        string nationalId, string mobileNumber, string iban) => new()
    {
        UserId = userId,
        FirstName = firstName,
        LastName = lastName,
        NationalId = nationalId,
        MobileNumber = mobileNumber,
        IBAN = iban
    };
}
