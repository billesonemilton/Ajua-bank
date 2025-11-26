// File: src/TransactionService/Program.cs
using AjuaBank.TransactionService.Data;
using AjuaBank.TransactionService.Services;
using AjuaBank.Shared.DTOs;
using AjuaBank.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IMessageBroker, RabbitMqService>();
builder.Services.AddScoped<TransactionProcessor>();

var app = builder.Build();

// Auto-migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();
    await db.Database.MigrateAsync();
}

app.MapPost("/api/transactions", async (CreateTransactionRequest req, HttpContext ctx, TransactionProcessor svc) =>
{
    var userIdClaim = ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        return Results.Unauthorized();

    var result = await svc.CreateTransactionAsync(userId, req);
    return result != null ? Results.Ok(result) : Results.BadRequest("Transaction failed");
});

app.MapGet("/api/transactions", async (HttpContext ctx, TransactionProcessor svc) =>
{
    var userIdClaim = ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        return Results.Unauthorized();

    var transactions = await svc.GetUserTransactionsAsync(userId);
    return Results.Ok(transactions);
});

app.MapGet("/", () => "Transaction Service is running");

app.Run();