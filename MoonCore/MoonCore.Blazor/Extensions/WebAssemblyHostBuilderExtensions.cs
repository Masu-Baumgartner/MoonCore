using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Blazor.Services;
using MoonCore.Helpers;
using MoonCore.Models;
using MoonCore.OAuth2.Requests;

namespace MoonCore.Blazor.Extensions;

public static class WebAssemblyHostBuilderExtensions
{
    public static void AddTokenAuthentication(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped(serviceProvider =>
        {
            var httpClient = serviceProvider.GetRequiredService<HttpClient>();
            var localStorageService = serviceProvider.GetRequiredService<LocalStorageService>();

            var httpApiClient = new HttpApiClient(httpClient);

            httpApiClient.OnConfigureRequest += async request =>
            {
                // Don't handle auth when calling an authentication endpoint
                if (request.RequestUri?.ToString().StartsWith("api/_auth") ?? false)
                    return;

                // Check expire date
                var expiresAt = await localStorageService.GetDefaulted("ExpiresAt", DateTime.MinValue);

                string accessToken;

                if (DateTime.UtcNow > expiresAt) // Expire date reached, refresh access token
                {
                    var refreshToken = await localStorageService.GetStringDefaulted("RefreshToken", "unset");

                    // Call to refresh provider
                    TokenPair refreshData;

                    try
                    {
                        refreshData = await httpApiClient.PostJson<TokenPair>("api/_auth/refresh",
                            new RefreshRequest()
                            {
                                RefreshToken = refreshToken
                            }
                        );
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    // Save new tokens
                    await localStorageService.SetString("AccessToken", refreshData.AccessToken);
                    await localStorageService.SetString("RefreshToken", refreshData.RefreshToken);
                    await localStorageService.Set("ExpiresAt", DateTime.UtcNow.AddSeconds(refreshData.ExpiresIn));

                    accessToken = refreshData.AccessToken;
                }
                else
                    accessToken = await localStorageService.GetStringDefaulted("AccessToken", "unset");

                request.Headers.Add("Authorization", $"Bearer {accessToken}");
            };

            return httpApiClient;
        });
    }

    public static void AddOAuth2(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<OAuth2FrontendService>();
    }
}