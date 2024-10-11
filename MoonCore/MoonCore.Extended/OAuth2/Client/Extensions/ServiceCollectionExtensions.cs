using Microsoft.Extensions.DependencyInjection;
using MoonCore.Extended.OAuth2.Client.Configuration;
using MoonCore.Extended.OAuth2.Client.Services;

namespace MoonCore.Extended.OAuth2.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMoonCoreOAuth2(this IServiceCollection collection,
        Action<OAuth2ClientConfiguration> onConfigure)
    {
        var configuration = new OAuth2ClientConfiguration();
        onConfigure.Invoke(configuration);
        
        collection.AddSingleton(sp => new OAuth2ClientService(configuration));
    }
}