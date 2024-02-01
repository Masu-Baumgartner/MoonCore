using System.Security.Cryptography;

namespace MoonCore.Helpers;

public static class HashHelper
{
    private const int DefaultIterations = 10000;

    private class HashVersion
    {
        public short Version { get; set; }
        public int SaltSize { get; set; }
        public int HashSize { get; set; }
    }

    private static readonly Dictionary<short, HashVersion> _versions = new Dictionary<short, HashVersion>
    {
        {
            1, new HashVersion
            {
                Version = 1,
                HashSize = 256 / 8,
                SaltSize = 128 / 8
            }
        }
    };

    private static HashVersion DefaultVersion => _versions[1];

    public static bool IsLatestHashVersion(byte[] data)
    {
        var version = BitConverter.ToInt16(data, 0);
        return version == DefaultVersion.Version;
    }

    public static bool IsLatestHashVersion(string data)
    {
        var dataBytes = Convert.FromBase64String(data);
        return IsLatestHashVersion(dataBytes);
    }

    public static byte[] GetRandomBytes(int length)
    {
        var data = new byte[length];
        using (var randomNumberGenerator = RandomNumberGenerator.Create())
        {
            randomNumberGenerator.GetBytes(data);
        }

        return data;
    }

    public static byte[] Hash(string clearText, int iterations = DefaultIterations)
    {
        var currentVersion = DefaultVersion;
        var saltBytes = GetRandomBytes(currentVersion.SaltSize);

        using (var pbkdf2 = new Rfc2898DeriveBytes(clearText, saltBytes, iterations))
        {
            var hashBytes = pbkdf2.GetBytes(currentVersion.HashSize);
            
            var resultBytes = new byte[2 + currentVersion.SaltSize + currentVersion.HashSize];
            BitConverter.GetBytes(currentVersion.Version).CopyTo(resultBytes, 0);
            saltBytes.CopyTo(resultBytes, 2);
            hashBytes.CopyTo(resultBytes, 2 + currentVersion.SaltSize);

            return resultBytes;
        }
    }

    public static string HashToString(string clearText, int iterations = DefaultIterations)
    {
        var data = Hash(clearText, iterations);
        return Convert.ToBase64String(data);
    }

    public static bool Verify(string clearText, byte[] data)
    {
        var currentVersion = _versions[BitConverter.ToInt16(data, 0)];
        var saltBytes = data.Skip(2).Take(currentVersion.SaltSize).ToArray();
        var hashBytes = data.Skip(2 + currentVersion.SaltSize).ToArray();

        using (var pbkdf2 = new Rfc2898DeriveBytes(clearText, saltBytes, BitConverter.ToInt32(data, 2)))
        {
            var verificationHashBytes = pbkdf2.GetBytes(currentVersion.HashSize);
            return hashBytes.SequenceEqual(verificationHashBytes);
        }
    }

    public static bool Verify(string clearText, string data)
    {
        var dataBytes = Convert.FromBase64String(data);
        return Verify(clearText, dataBytes);
    }
}