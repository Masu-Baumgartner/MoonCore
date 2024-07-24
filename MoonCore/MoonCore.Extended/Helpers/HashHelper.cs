namespace MoonCore.Extended.Helpers;

public static class HashHelper
{
    public static string Hash(string clearText) => BCrypt.Net.BCrypt.EnhancedHashPassword(clearText);

    public static bool Verify(string clearText, string data) => BCrypt.Net.BCrypt.EnhancedVerify(clearText, data);
}