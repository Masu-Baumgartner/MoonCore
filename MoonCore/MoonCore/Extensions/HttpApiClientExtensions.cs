using MoonCore.Helpers;

namespace MoonCore.Extensions;

public static class HttpApiClientExtensions
{
    public static void UseBearerTokenConsumer(this HttpApiClient client, TokenConsumer tokenConsumer)
    {
        client.OnConfigureRequest += async request =>
        {
            var accessToken = await tokenConsumer.GetAccessToken();
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
        };
    }
    
    public static void UseBearerTokenConsumer(this HttpApiClient client, Func<Task<TokenConsumer>> onGetTokenConsumer)
    {
        client.OnConfigureRequest += async request =>
        {
            var tokenConsumer = await onGetTokenConsumer.Invoke();
            
            var accessToken = await tokenConsumer.GetAccessToken();
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
        };
    }
}