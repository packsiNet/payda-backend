using PayDa.Domain.Common;

namespace PayDa.Domain.Entities;

public class SystemConfig : BaseEntity
{
    public int MatchConfirmationHours { get; private set; } = 24;

    private SystemConfig() { }

    public static SystemConfig Create() => new() { MatchConfirmationHours = 24 };

    public void SetMatchConfirmationHours(int hours)
    {
        MatchConfirmationHours = hours;
        UpdatedAt = DateTime.UtcNow;
    }
}
