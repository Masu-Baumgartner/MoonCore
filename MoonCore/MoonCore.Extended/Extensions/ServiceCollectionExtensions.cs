using Microsoft.Extensions.DependencyInjection;
using MoonCore.Extended.Configuration;
using MoonCore.Extended.Helpers;
using MoonCore.Extended.OAuth2.Consumer;
using MoonCore.Extended.OAuth2.Provider;

namespace MoonCore.Extended.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddOAuth2Provider(this IServiceCollection collection, Action<OAuth2ProviderConfiguration> onConfigure)
    {
        collection.AddSingleton(sp =>
        {
            var config = new OAuth2ProviderConfiguration();
            onConfigure.Invoke(config);

            return new OAuth2ProviderService(config);
        });
    }

    public static void AddOAuth2Consumer(this IServiceCollection collection,
        Action<OAuth2ConsumerConfiguration> onConfigure)
    {
        collection.AddSingleton(sp =>
        {
            var config = new OAuth2ConsumerConfiguration();
            onConfigure.Invoke(config);
            
            var httpClient = sp.GetRequiredService<HttpClient>();

            return new OAuth2ConsumerService(config, httpClient);
        });
    }

    public static void AddOAuth2Consumer(this IServiceCollection collection, HttpClient httpClient,
        Action<OAuth2ConsumerConfiguration> onConfigure)
    {
        collection.AddSingleton(_ =>
        {
            var config = new OAuth2ConsumerConfiguration();
            onConfigure.Invoke(config);

            return new OAuth2ConsumerService(config, httpClient);
        });
    }

    public static void AddTokenAuthentication(this IServiceCollection collection, Action<TokenAuthenticationConfiguration> onConfigure)
    {
        var configuration = new TokenAuthenticationConfiguration();
        onConfigure.Invoke(configuration);

        collection.AddSingleton(configuration);
    }
}