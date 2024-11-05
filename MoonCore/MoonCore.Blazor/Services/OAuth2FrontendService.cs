using Microsoft.Extensions.Logging;
using MoonCore.Helpers;
using MoonCore.Http.Requests.OAuth2;
using MoonCore.Http.Responses.OAuth2;

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
        var startResult = await HttpApiClient.GetJson<OAuth2StartResponse>("/api/_auth/oauth2/start");

        return $"{startResult.Endpoint}?client_id={startResult.ClientId}&redirect_uri={startResult.RedirectUri}";
    }

    public async Task<bool> Complete(string code)
    {
        try
        {
            var completeResult = await HttpApiClient.PostJson<OAuth2CompleteResponse>("/api/_auth/oauth2/complete",
                new OAuth2CompleteRequest()
                {
                    Code = code
                }
            );

            await StorageService.SetString("AccessToken", completeResult.AccessToken);
            await StorageService.SetString("RefreshToken", completeResult.RefreshToken);
            await StorageService.Set("ExpiresAt", completeResult.ExpiresAt);

            return true;
        }
        catch (Exception e)
        {
            Logger.LogError("An error occured while completing oauth2 workflow: {e}", e);
            return false;
        }
    }
}