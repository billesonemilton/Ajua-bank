
// File: src/SharedLibrary/Models/FraudAlert.cs
namespace AjuaBank.Shared.Models;

public class FraudAlert
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public double RiskScore { get; set; }
    public string RiskLevel { get; set; } = "Low"; // Low, Medium, High, Critical
    public string Reasons { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    public bool IsResolved { get; set; } = false;
}
