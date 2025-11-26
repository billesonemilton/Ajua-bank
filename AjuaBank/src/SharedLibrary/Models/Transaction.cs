
// File: src/SharedLibrary/Models/Transaction.cs
namespace AjuaBank.Shared.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty; // Deposit, Withdrawal, Transfer
    public decimal Amount { get; set; }
    public string? ToAccountNumber { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Flagged
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public double FraudScore { get; set; } = 0.0;
}
