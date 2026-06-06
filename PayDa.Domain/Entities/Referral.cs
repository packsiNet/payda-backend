using PayDa.Domain.Common;

namespace PayDa.Domain.Entities;

public class Referral : BaseEntity
{
    public Guid ReferrerId { get; private set; }
    public User Referrer { get; private set; } = default!;

    public Guid ReferredId { get; private set; }
    public User Referred { get; private set; } = default!;

    private Referral() { }

    public static Referral Create(Guid referrerId, Guid referredId) => new()
    {
        ReferrerId = referrerId,
        ReferredId = referredId
    };
}
