using MoonCore.Extended.Models;

namespace MoonCore.Extended.Helpers;

public class TokenHelper
{
    private readonly JwtHelper JwtHelper;

    public TokenHelper(JwtHelper jwtHelper)
    {
        JwtHelper = jwtHelper;
    }

    public async Task<TokenPair> GeneratePair(
        string secret,
        Action<Dictionary<string, string>> onConfigure,
        int accessDuration = 60,
        int renewDuration = 3600
    )
    {
        var accessToken = await JwtHelper.Create(secret, onConfigure, TimeSpan.FromSeconds(accessDuration));
        var refreshToken = await JwtHelper.Create(secret, onConfigure, TimeSpan.FromSeconds(renewDuration));

        return new TokenPair()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<bool> IsValidAccessToken(string accessToken, string secret,
        Func<Dictionary<string, string>, bool> validateData)
    {
        if (!await JwtHelper.Validate(secret, accessToken))
            return false;

        var data = await JwtHelper.Decode(accessToken);

        return validateData.Invoke(data);
    }

    public async Task<TokenPair?> RefreshPair(
        string refreshToken,
        string secret,
        Func<Dictionary<string, string>, Dictionary<string, string>, bool> processData,
        int accessDuration = 60,
        int renewDuration = 3600
    )
    {
        if (!await JwtHelper.Validate(secret, refreshToken))
            return null;

        var data = await JwtHelper.Decode(refreshToken);
        var newData = new Dictionary<string, string>();

        if (!processData.Invoke(data, newData))
            return null;

        var newAccessToken = await JwtHelper.Create(secret, newData, TimeSpan.FromSeconds(accessDuration));
        var newRefreshToken = await JwtHelper.Create(secret, newData, TimeSpan.FromSeconds(renewDuration));

        return new TokenPair()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}