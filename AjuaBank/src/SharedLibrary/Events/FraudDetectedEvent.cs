// File: src/SharedLibrary/Events/FraudDetectedEvent.cs
namespace AjuaBank.Shared.Events;

public record FraudDetectedEvent(
    Guid TransactionId,
    double RiskScore,
    string RiskLevel,
    string[] Reasons
);