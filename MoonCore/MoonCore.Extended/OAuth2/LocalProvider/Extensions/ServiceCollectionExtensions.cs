using Microsoft.Extensions.DependencyInjection;
using MoonCore.Extended.OAuth2.Consumer;
using MoonCore.Extended.OAuth2.LocalProvider.Implementations;
using MoonCore.Extensions;

namespace MoonCore.Extended.OAuth2.LocalProvider.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddLocalOAuth2Provider<T>(this IServiceCollection collection, string publicUrl,
        Action<LocalProviderConfiguration>? onConfigure = null) where T : IUserModel
    {
        var authConfig = collection.FindRegisteredInstance<AuthenticationConfiguration<T>>();

        if (authConfig == null)
            throw new ArgumentNullException(nameof(AuthenticationConfiguration<T>), "You need to add oauth2 authentication before trying to add the local provider");

        var configuration = new LocalProviderConfiguration()
        {
            ClientId = authConfig.ClientId,
            ClientSecret = authConfig.ClientSecret,
            RedirectUri = authConfig.RedirectUri,
            AccessSecret = authConfig.AccessSecret,
            RefreshSecret = authConfig.RefreshSecret,
            RefreshDuration = authConfig.RefreshDuration,
            RefreshInterval = authConfig.RefreshInterval,
            PublicUrl = publicUrl
        };
        
        onConfigure?.Invoke(configuration);
        
        collection.AddSingleton(configuration);
        collection.AddSingleton<LocalProviderService<T>>();

        collection.AddScoped<IOAuth2Provider<T>, LocalOAuth2Provider<T>>();
    }
}