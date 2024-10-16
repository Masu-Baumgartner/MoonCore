using MoonCore.Extended.Models;

namespace MoonCore.Extended.Helpers;

public class TokenConsumer
{
    private string AccessToken;
    private string RefreshToken;
    private DateTime ExpireTimestamp;

    private Func<string, Task<TokenPair>> RefreshFunction;
    
    public TokenConsumer(string accessToken, string refreshToken, DateTime expireTimestamp, Func<string, Task<TokenPair>> refreshFunc)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpireTimestamp = expireTimestamp;

        RefreshFunction = refreshFunc;
    }

    public async Task<string> GetAccessToken()
    {
        if (DateTime.UtcNow > ExpireTimestamp)
        {
            var pair = await RefreshFunction.Invoke(RefreshToken);

            AccessToken = pair.AccessToken;
            RefreshToken = pair.RefreshToken;
        }

        return AccessToken;
    }
}