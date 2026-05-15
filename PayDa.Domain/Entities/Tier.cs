using PayDa.Domain.Common;

namespace PayDa.Domain.Entities;

public class Tier : BaseEntity
{
    public string Name { get; private set; } = default!;
    public int Order { get; private set; }
    public int MaxActiveRequests { get; private set; }
    public decimal MaxAmountPerRequest { get; private set; }
    public int RequiredCompletedTransactions { get; private set; }

    private Tier() { }

    public static Tier Create(string name, int order, int maxActiveRequests,
        decimal maxAmountPerRequest, int requiredCompletedTransactions) => new()
    {
        Name = name,
        Order = order,
        MaxActiveRequests = maxActiveRequests,
        MaxAmountPerRequest = maxAmountPerRequest,
        RequiredCompletedTransactions = requiredCompletedTransactions
    };

    public void Update(string name, int maxActiveRequests,
        decimal maxAmountPerRequest, int requiredCompletedTransactions)
    {
        Name = name;
        MaxActiveRequests = maxActiveRequests;
        MaxAmountPerRequest = maxAmountPerRequest;
        RequiredCompletedTransactions = requiredCompletedTransactions;
        UpdatedAt = DateTime.UtcNow;
    }
}
