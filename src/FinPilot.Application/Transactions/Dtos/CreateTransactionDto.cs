using FinPilot.Domain.Entities;

namespace FinPilot.Application.Transactions.Dtos;

public class CreateTransactionDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime? TransactionDate { get; set; }
}
