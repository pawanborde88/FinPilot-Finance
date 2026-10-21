using FinPilot.Domain.Common;

namespace FinPilot.Domain.Entities;

public enum TransactionType
{
    Income = 1,
    Expense = 2
}

public class Transaction : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
}
