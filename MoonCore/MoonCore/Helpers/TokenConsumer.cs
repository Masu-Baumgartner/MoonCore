using MoonCore.Models;

namespace MoonCore.Helpers;

public class TokenConsumer
{
    private TokenPair CurrentPair;
    private DateTime ExpireTimestamp;

    private Func<string, Task<(TokenPair, DateTime)>> RefreshFunction;

    public TokenConsumer(Func<string, Task<(TokenPair, DateTime)>> refreshFunction)
    {
        RefreshFunction = refreshFunction;
        CurrentPair = new()
        {
            AccessToken = "unset",
            RefreshToken = "unset"
        };
        
        ExpireTimestamp = DateTime.UtcNow;
    }

    public TokenConsumer(TokenPair pair, DateTime expireTimestamp, Func<string, Task<(TokenPair, DateTime)>> refreshFunction)
    {
        CurrentPair = pair;
        ExpireTimestamp = expireTimestamp;
        RefreshFunction = refreshFunction;
    }

    public async Task<string> GetAccessToken()
    {
        if (DateTime.UtcNow >= ExpireTimestamp)
        {
            var result = await RefreshFunction.Invoke(CurrentPair.RefreshToken);

            CurrentPair = result.Item1;
            ExpireTimestamp = result.Item2;
        }

        return CurrentPair.AccessToken;
    }
}