using Microsoft.Extensions.Logging;
using MoonCore.Helpers;
using MoonCore.Models;
using MoonCore.OAuth2.Requests;
using MoonCore.OAuth2.Responses;

namespace MoonCore.Blazor.Services;

public class OAuth2FrontendService
{
    private readonly HttpApiClient HttpApiClient;
    private readonly ILogger<OAuth2FrontendService> Logger;
    private readonly LocalStorageService StorageService;

    public OAuth2FrontendService(
        HttpApiClient httpApiClient,
        ILogger<OAuth2FrontendService> logger,
        LocalStorageService storageService
    )
    {
        HttpApiClient = httpApiClient;
        Logger = logger;
        StorageService = storageService;
    }

    public async Task<string> Start()
    {
        var startResult = await HttpApiClient.GetJson<StartResponse>("api/_auth/start");

        return $"{startResult.Endpoint}" +
               $"?client_id={startResult.ClientId}" +
               $"&redirect_uri={startResult.RedirectUri}" +
               $"&response_type=code";
    }

    public async Task<bool> Complete(string code)
    {
        try
        {
            var completeResult = await HttpApiClient.PostJson<TokenPair>("api/_auth/complete",
                new CompleteRequest()
                {
                    Code = code
                }
            );

            await StorageService.SetString("AccessToken", completeResult.AccessToken);
            await StorageService.SetString("RefreshToken", completeResult.RefreshToken);
            await StorageService.Set("ExpiresAt", DateTime.UtcNow.AddSeconds(completeResult.ExpiresIn));

            return true;
        }
        catch (Exception e)
        {
            Logger.LogError("An error occured while completing oauth2 workflow: {e}", e);
            return false;
        }
    }
}