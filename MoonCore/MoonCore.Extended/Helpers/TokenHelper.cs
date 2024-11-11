using System.Text.Json;
using MoonCore.Models;

namespace MoonCore.Extended.Helpers;

public static class TokenHelper
{
    public static TokenPair GeneratePair(
        string accessSecret,
        string refreshSecret,
        Action<Dictionary<string, object>> onConfigure,
        int accessDuration = 60,
        int renewDuration = 3600
    )
    {
        var data = new Dictionary<string, object>();
        onConfigure.Invoke(data);
        
        return GeneratePair(accessSecret, refreshSecret, data, accessDuration, renewDuration);
    }
    
    public static TokenPair GeneratePair(
        string accessSecret,
        string refreshSecret,
        Dictionary<string, object> data,
        int accessDuration = 60,
        int renewDuration = 3600
    )
    {
        var accessToken = JwtHelper.Encode(accessSecret, data, TimeSpan.FromSeconds(accessDuration));
        var refreshToken = JwtHelper.Encode(refreshSecret, data, TimeSpan.FromSeconds(renewDuration));

        return new TokenPair()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = accessDuration
        };
    }

    public static bool IsValidToken(
        string token,
        string secret,
        Func<Dictionary<string, JsonElement>, bool> validateData)
    {
        if (!JwtHelper.TryVerifyAndDecodePayload(secret, token, out var data))
            return false;

        return validateData.Invoke(data);
    }

    public static Dictionary<string, JsonElement> DecodeToken(string token) => JwtHelper.DecodePayload(token);
}