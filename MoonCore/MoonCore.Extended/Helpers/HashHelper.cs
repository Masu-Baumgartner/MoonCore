namespace MoonCore.Extended.Helpers;

/// <summary>
/// Helper for hashing string content using bcrypt
/// </summary>
public static class HashHelper
{
    /// <summary>
    /// Hashes the provided text using BCrypt's enhanced hashing 
    /// </summary>
    /// <param name="clearText">Text to hash</param>
    /// <returns>Hash string</returns>
    public static string Hash(string clearText) => BCrypt.Net.BCrypt.EnhancedHashPassword(clearText);

    /// <summary>
    /// Verifies if the provided text matches with the hash
    /// </summary>
    /// <param name="clearText">Text to compare to the hash</param>
    /// <param name="hash">Hash to compare the text to</param>
    /// <returns>If it matches, it returns true otherwise false</returns>
    public static bool Verify(string clearText, string hash) => BCrypt.Net.BCrypt.EnhancedVerify(clearText, hash);
}