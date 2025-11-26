
// File: src/TransactionService/Services/TransactionProcessor.cs
using AjuaBank.Shared.DTOs;
using AjuaBank.Shared.Events;
using AjuaBank.Shared.Interfaces;
using AjuaBank.Shared.Models;
using AjuaBank.TransactionService.Data;
using Microsoft.EntityFrameworkCore;

namespace AjuaBank.TransactionService.Services;

public class TransactionProcessor
{
    private readonly TransactionDbContext _context;
    private readonly IMessageBroker _broker;

    public TransactionProcessor(TransactionDbContext context, IMessageBroker broker)
    {
        _context = context;
        _broker = broker;
    }

    public async Task<TransactionResponse?> CreateTransactionAsync(Guid userId, CreateTransactionRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        // Validate balance for withdrawals/transfers
        if ((request.Type == "Withdrawal" || request.Type == "Transfer") && user.AccountBalance < request.Amount)
            return null;

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = request.Type,
            Amount = request.Amount,
            ToAccountNumber = request.ToAccountNumber,
            Description = request.Description,
            Status = "Pending",
            Timestamp = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);

        // Update balance immediately for deposits, hold for others pending fraud check
        if (request.Type == "Deposit")
        {
            user.AccountBalance += request.Amount;
            transaction.Status = "Completed";
        }
        else
        {
            user.AccountBalance -= request.Amount; // Deduct immediately, will refund if fraud detected
        }

        await _context.SaveChangesAsync();

        // Publish event for fraud detection
        var evt = new TransactionCreatedEvent(
            transaction.Id,
            userId,
            request.Type,
            request.Amount,
            transaction.Timestamp,
            request.ToAccountNumber
        );

        await _broker.PublishAsync("transactions", "transaction.created", evt);

        return new TransactionResponse(
            transaction.Id,
            transaction.Type,
            transaction.Amount,
            transaction.Status,
            transaction.Timestamp,
            0.0
        );
    }

    public async Task<List<TransactionResponse>> GetUserTransactionsAsync(Guid userId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Timestamp)
            .Take(50)
            .ToListAsync();

        return transactions.Select(t => new TransactionResponse(
            t.Id,
            t.Type,
            t.Amount,
            t.Status,
            t.Timestamp,
            t.FraudScore
        )).ToList();
    }

    public async Task UpdateFraudScoreAsync(Guid transactionId, double score, string riskLevel)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId);
        if (transaction == null) return;

        transaction.FraudScore = score;

        if (riskLevel == "High" || riskLevel == "Critical")
        {
            transaction.Status = "Flagged";
            // Refund if flagged
            var user = await _context.Users.FindAsync(transaction.UserId);
            if (user != null && transaction.Type != "Deposit")
            {
                user.AccountBalance += transaction.Amount;
            }
        }
        else
        {
            transaction.Status = "Completed";
        }

        await _context.SaveChangesAsync();
    }
}