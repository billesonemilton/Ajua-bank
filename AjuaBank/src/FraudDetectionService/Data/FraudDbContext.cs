// File: src/FraudDetectionService/Data/FraudDbContext.cs
using Microsoft.EntityFrameworkCore;
using AjuaBank.Shared.Models;

namespace AjuaBank.FraudDetectionService.Data;

public class FraudDbContext : DbContext
{
    public FraudDbContext(DbContextOptions<FraudDbContext> options) : base(options) { }
    
    public DbSet<FraudAlert> FraudAlerts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FraudAlert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TransactionId);
        });
    }
}