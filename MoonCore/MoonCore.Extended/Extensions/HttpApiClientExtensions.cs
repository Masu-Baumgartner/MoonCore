using MoonCore.Extended.Helpers;
using MoonCore.Helpers;

namespace MoonCore.Extended.Extensions;

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
}