// File: src/AuthService/Data/AuthDbContext.cs
using Microsoft.EntityFrameworkCore;
using AjuaBank.Shared.Models;

namespace AjuaBank.AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.AccountNumber).IsUnique();
        });
    }
}