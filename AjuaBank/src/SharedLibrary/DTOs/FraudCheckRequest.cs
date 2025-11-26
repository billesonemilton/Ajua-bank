

// File: src/SharedLibrary/DTOs/FraudCheckRequest.cs
namespace AjuaBank.Shared.DTOs;

public record FraudCheckRequest(
    Guid TransactionId,
    Guid UserId,
    decimal Amount,
    string Type,
    DateTime Timestamp,
    string? ToAccountNumber
);

public record FraudCheckResponse(
    Guid TransactionId,
    double RiskScore,
    string RiskLevel,
    string[] Reasons,
    bool ShouldBlock
);