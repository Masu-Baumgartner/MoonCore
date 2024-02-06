using System.Security.Cryptography;

namespace MoonCore.Helpers;

public static class HashHelper
{
    public static string HashToString(string clearText)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(clearText);
    }

    public static bool Verify(string clearText, string data)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(clearText, data);
    }
}