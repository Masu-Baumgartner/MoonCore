using Microsoft.Extensions.DependencyInjection;
using MoonCore.Extended.Helpers;

namespace MoonCore.Extended.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddOAuth2Provider(this IServiceCollection collection, Action<OAuth2.AuthServer.OAuth2Configuration> onConfigure)
    {
        collection.AddSingleton(sp =>
        {
            var config = new OAuth2.AuthServer.OAuth2Configuration();
            onConfigure.Invoke(config);
            
            var jwtHelper = sp.GetRequiredService<JwtHelper>();
            var tokenHelper = sp.GetRequiredService<TokenHelper>();

            return new OAuth2.AuthServer.OAuth2Service(config, jwtHelper, tokenHelper);
        });
    }

    public static void AddOAuth2Consumer(this IServiceCollection collection,
        Action<OAuth2.ApiServer.OAuth2Configuration> onConfigure)
    {
        collection.AddSingleton(sp =>
        {
            var config = new OAuth2.ApiServer.OAuth2Configuration();
            onConfigure.Invoke(config);
            
            var httpClient = sp.GetRequiredService<HttpClient>();

            return new OAuth2.ApiServer.OAuth2Service(config, httpClient);
        });
    }

    public static void AddOAuth2Consumer(this IServiceCollection collection, HttpClient httpClient,
        Action<OAuth2.ApiServer.OAuth2Configuration> onConfigure)
    {
        collection.AddSingleton(_ =>
        {
            var config = new OAuth2.ApiServer.OAuth2Configuration();
            onConfigure.Invoke(config);

            return new OAuth2.ApiServer.OAuth2Service(config, httpClient);
        });
    }
}