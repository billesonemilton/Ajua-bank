// File: src/SharedLibrary/Interfaces/IMessageBroker.cs
namespace AjuaBank.Shared.Interfaces;

public interface IMessageBroker
{
    Task PublishAsync<T>(string exchange, string routingKey, T message);
    Task SubscribeAsync<T>(string queue, Func<T, Task> handler);
}

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

// File: src/SharedLibrary/Events/FraudDetectedEvent.cs
namespace AjuaBank.Shared.Events;

public record FraudDetectedEvent(
    Guid TransactionId,
    double RiskScore,
    string RiskLevel,
    string[] Reasons
);

// File: src/SharedLibrary/Utilities/JwtHelper.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AjuaBank.Shared.Utilities;

public static class JwtHelper
{
    public static string GenerateToken(Guid userId, string email, string role, string secretKey, int expiryHours = 24)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddHours(expiryHours),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static ClaimsPrincipal? ValidateToken(string token, string secretKey)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        
        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            
            return principal;
        }
        catch
        {
            return null;
        }
    }
}

// File: src/SharedLibrary/Utilities/PasswordHelper.cs
using System.Security.Cryptography;

namespace AjuaBank.Shared.Utilities;

public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}

// File: src/SharedLibrary/SharedLibrary.csproj
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>AjuaBank.Shared</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.3.1" />
    <PackageReference Include="Microsoft.IdentityModel.Tokens" Version="7.3.1" />
    <PackageReference Include="RabbitMQ.Client" Version="6.8.1" />
  </ItemGroup>
</Project>