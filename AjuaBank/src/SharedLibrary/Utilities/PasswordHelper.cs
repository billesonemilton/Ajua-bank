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