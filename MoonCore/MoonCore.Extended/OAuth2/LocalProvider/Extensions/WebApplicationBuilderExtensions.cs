using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Extended.OAuth2.Consumer;
using MoonCore.Extensions;

namespace MoonCore.Extended.OAuth2.LocalProvider.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddLocalOAuth2Provider<T>(this WebApplicationBuilder builder, string publicUrl, Action<LocalProviderConfiguration>? onConfigure = null) where T : IUserModel
    {
        var authConfig = builder.Services.FindRegisteredInstance<AuthenticationConfiguration<T>>();

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
        
        builder.Services.AddSingleton(configuration);
        builder.Services.AddSingleton<LocalProviderService<T>>();
    }
}