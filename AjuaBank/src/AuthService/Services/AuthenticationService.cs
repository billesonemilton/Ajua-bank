
// File: src/AuthService/Services/AuthenticationService.cs
using AjuaBank.Shared.DTOs;
using AjuaBank.Shared.Models;
using AjuaBank.Shared.Utilities;
using AjuaBank.AuthService.Data;
using Microsoft.EntityFrameworkCore;

namespace AjuaBank.AuthService.Services;

public class AuthenticationService
{
    private readonly AuthDbContext _context;
    private readonly IConfiguration _config;

    public AuthenticationService(AuthDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = PasswordHelper.HashPassword(request.Password),
            FullName = request.FullName,
            Role = "Customer",
            AccountNumber = GenerateAccountNumber(),
            AccountBalance = 0,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = JwtHelper.GenerateToken(user.Id, user.Email, user.Role, 
            _config["Jwt:Secret"] ?? "your-secret-key-min-32-chars-long!");

        return new AuthResponse(token, user.Email, user.Role);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        
        if (user == null || !user.IsActive)
            return null;

        if (!PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
            return null;

        var token = JwtHelper.GenerateToken(user.Id, user.Email, user.Role,
            _config["Jwt:Secret"] ?? "your-secret-key-min-32-chars-long!");

        return new AuthResponse(token, user.Email, user.Role);
    }

    private string GenerateAccountNumber()
    {
        return $"AJB{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{Random.Shared.Next(1000, 9999)}";
    }
}
