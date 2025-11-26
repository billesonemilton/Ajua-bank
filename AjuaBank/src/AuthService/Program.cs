// File: src/AuthService/Program.cs
using AjuaBank.AuthService.Data;
using AjuaBank.AuthService.Services;
using AjuaBank.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<AuthenticationService>();

var app = builder.Build();

// Auto-migrate DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    await db.Database.MigrateAsync();
}

app.MapControllers();

app.MapPost("/api/auth/register", async (RegisterRequest req, AuthenticationService svc) =>
{
    var result = await svc.RegisterAsync(req);
    return result != null ? Results.Ok(result) : Results.BadRequest("Registration failed");
});

app.MapPost("/api/auth/login", async (LoginRequest req, AuthenticationService svc) =>
{
    var result = await svc.LoginAsync(req);
    return result != null ? Results.Ok(result) : Results.Unauthorized();
});

app.Run();
