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
            RefreshToken = refreshToken
        };
    }

    public static bool IsValidAccessToken(
        string accessToken,
        string secret,
        Func<Dictionary<string, JsonElement>, bool> validateData)
    {
        if (!JwtHelper.TryVerifyAndDecodePayload(secret, accessToken, out var data))
            return false;

        return validateData.Invoke(data);
    }

    public static TokenPair? RefreshPair(
        string refreshToken,
        string accessSecret,
        string refreshSecret,
        Func<Dictionary<string, JsonElement>, Dictionary<string, object>, bool> processData,
        int accessDuration = 60,
        int renewDuration = 3600
    )
    {
        if (!JwtHelper.TryVerifyAndDecodePayload(refreshSecret, refreshToken, out var data))
            return null;

        var newData = new Dictionary<string, object>();

        if (!processData.Invoke(data, newData))
            return null;

        var newAccessToken = JwtHelper.Encode(accessSecret, newData, TimeSpan.FromSeconds(accessDuration));
        var newRefreshToken = JwtHelper.Encode(refreshSecret, newData, TimeSpan.FromSeconds(renewDuration));

        return new TokenPair()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}