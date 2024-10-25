using MoonCore.Blazor.Services;
using MoonCore.Helpers;
using MoonCore.Models;

namespace MoonCore.Blazor.Extensions;

public static class HttpApiClientExtensions
{
    public static void AddLocalStorageTokenAuthentication(
        this HttpApiClient httpApiClient,
        LocalStorageService localStorageService,
        Func<string, Task<(TokenPair, DateTime)>> refreshFunction,
        string accessTokenName = "AccessToken",
        string refreshTokenName = "RefreshToken",
        string expiresAtName = "ExpiresAt",
        bool enableCache = false)
    {
        TokenConsumer? consumer = null;

        httpApiClient.OnConfigureRequest += async request =>
        {
            string accessToken;

            if (enableCache)
            {
                if (consumer == null)
                {
                    var tokenPair = new TokenPair()
                    {
                        AccessToken = await localStorageService.GetStringDefaulted(accessTokenName, "unset"),
                        RefreshToken = await localStorageService.GetStringDefaulted(refreshTokenName, "unset")
                    };

                    var expiresAt = await localStorageService.GetDefaulted(expiresAtName, DateTime.MinValue);

                    consumer = new TokenConsumer(tokenPair, expiresAt, async refreshToken =>
                    {
                        var result = await refreshFunction.Invoke(refreshToken);

                        await localStorageService.SetString(accessTokenName, result.Item1.AccessToken);
                        await localStorageService.SetString(refreshTokenName, result.Item1.RefreshToken);
                        await localStorageService.Set(expiresAtName, result.Item2);

                        return result;
                    });
                }

                accessToken = await consumer.GetAccessToken();
            }
            else
            {
                // Check expire date
                var expiresAt = await localStorageService.GetDefaulted(expiresAtName, DateTime.MinValue);

                if (DateTime.UtcNow > expiresAt) // Expire date reached, refresh access token
                {
                    var refreshToken = await localStorageService.GetStringDefaulted(refreshTokenName, "unset");

                    // Call to refresh provider
                    var result = await refreshFunction.Invoke(refreshToken);

                    // Save new tokens
                    await localStorageService.SetString(accessTokenName, result.Item1.AccessToken);
                    await localStorageService.SetString(refreshTokenName, result.Item1.RefreshToken);
                    await localStorageService.Set(expiresAtName, result.Item2);

                    accessToken = result.Item1.AccessToken;
                }
                else
                    accessToken = await localStorageService.GetStringDefaulted(accessTokenName, "unset");
            }

            
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
        };
    }
}