// File: src/SharedLibrary/Events/TransactionCreatedEvent.cs
namespace AjuaBank.Shared.Events;

public record TransactionCreatedEvent(
    Guid TransactionId,
    Guid UserId,
    string Type,
    decimal Amount,
    DateTime Timestamp,
    string? ToAccountNumber
);