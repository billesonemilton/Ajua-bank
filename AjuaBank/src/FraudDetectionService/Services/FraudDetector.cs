using System;
using System.Collections.Generic;
using System.Linq;                     // REQUIRED
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;   // REQUIRED for ToListAsync(), FindAsync()
using AjuaBank.Shared.DTOs;
using AjuaBank.Shared.Models;
using AjuaBank.FraudDetectionService.Data;

namespace AjuaBank.FraudDetectionService.Services;

public class FraudDetector
{
    private readonly FraudDbContext _context;

    public FraudDetector(FraudDbContext context)
    {
        _context = context;
    }

    public async Task<FraudCheckResponse> AnalyzeTransactionAsync(FraudCheckRequest request)
    {
        var reasons = new List<string>();
        double score = 0.0;

        if (request.Amount > 10000)
        {
            score += 30;
            reasons.Add("Large transaction amount");
        }

        if (request.Amount > 50000)
        {
            score += 40;
            reasons.Add("Extremely large transaction");
        }

        var hour = request.Timestamp.Hour;
        if (hour >= 22 || hour < 6)
        {
            score += 15;
            reasons.Add("Transaction during unusual hours");
        }

        var recentTransactions = await _context.Transactions
            .Where(t => t.UserId == request.UserId &&
                        t.Timestamp > request.Timestamp.AddMinutes(-10))
            .CountAsync(); // MUST be CountAsync()

        if (recentTransactions > 3)
        {
            score += 25;
            reasons.Add("Multiple rapid transactions detected");
        }

        if (request.Type == "Withdrawal" || request.Type == "Transfer")
        {
            score += 10;
            reasons.Add("High-risk transaction type");
        }

        string riskLevel = score switch
        {
            >= 75 => "Critical",
            >= 50 => "High",
            >= 25 => "Medium",
            _ => "Low"
        };

        bool shouldBlock = score >= 75;

        if (score >= 25)
        {
            var alert = new FraudAlert
            {
                Id = Guid.NewGuid(),
                TransactionId = request.TransactionId,
                RiskScore = score,
                RiskLevel = riskLevel,
                Reasons = string.Join("; ", reasons),
                DetectedAt = DateTime.UtcNow,
                IsResolved = false
            };

            _context.FraudAlerts.Add(alert);
            await _context.SaveChangesAsync();
        }

        return new FraudCheckResponse(
            request.TransactionId,
            score,
            riskLevel,
            reasons.ToArray(),
            shouldBlock
        );
    }

    public async Task<List<FraudAlert>> GetUnresolvedAlertsAsync()
    {
        return await _context.FraudAlerts
            .Where(a => !a.IsResolved)
            .OrderByDescending(a => a.RiskScore)
            .Take(100)
            .ToListAsync();
    }

    public async Task ResolveAlertAsync(Guid alertId)
    {
        var alert = await _context.FraudAlerts.FindAsync(alertId);
        if (alert != null)
        {
            alert.IsResolved = true;
            await _context.SaveChangesAsync();
        }
    }
}
