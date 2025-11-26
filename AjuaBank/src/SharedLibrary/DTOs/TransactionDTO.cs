
// File: src/SharedLibrary/DTOs/TransactionDTOs.cs
namespace AjuaBank.Shared.DTOs;

public record CreateTransactionRequest(
    string Type,
    decimal Amount,
    string? ToAccountNumber,
    string? Description
);

public record TransactionResponse(
    Guid Id,
    string Type,
    decimal Amount,
    string Status,
    DateTime Timestamp,
    double FraudScore
);