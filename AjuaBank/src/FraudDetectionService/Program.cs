using AjuaBank.FraudDetectionService.Data;
using AjuaBank.FraudDetectionService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FraudDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<FraudDetector>();
builder.Services.AddHostedService<TransactionConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FraudDbContext>();
    await db.Database.MigrateAsync();
}

app.MapGet("/api/fraud/alerts", async (FraudDetector detector) =>
{
    var alerts = await detector.GetUnresolvedAlertsAsync();
    return Results.Ok(alerts);
});

app.MapPost("/api/fraud/alerts/{id}/resolve", async (Guid id, FraudDetector detector) =>
{
    await detector.ResolveAlertAsync(id);
    return Results.Ok();
});

app.MapGet("/", () => "Fraud Detection Service is running");

app.Run();
