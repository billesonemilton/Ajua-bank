
// File: src/SharedLibrary/DTOs/AuthDTOs.cs
namespace AjuaBank.Shared.DTOs;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string FullName);
public record AuthResponse(string Token, string Email, string Role);
