using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Extended.Abstractions;
using MoonCore.Extended.Configuration;
using MoonCore.Extended.OAuth2.Consumer;
using MoonCore.Extended.OAuth2.Provider;
using MoonCore.Extensions;
using MoonCore.Http.Responses.OAuth2;

namespace MoonCore.Extended.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddTokenAuthentication(
        this WebApplicationBuilder builder,
        Action<TokenAuthenticationConfig> onConfigure
    )
    {
        var config = new TokenAuthenticationConfig();
        onConfigure.Invoke(config);

        builder.Services.AddSingleton(config);
    }

    public static void AddOAuth2Consumer(this WebApplicationBuilder builder, Action<OAuth2ConsumerConfiguration> onConfigure)
    {
        OAuth2ConsumerConfiguration configuration = new();
        onConfigure.Invoke(configuration);

        builder.Services.AddSingleton(configuration);
        builder.Services.AddSingleton<OAuth2ConsumerService>();
    }
    
    public static void AddOAuth2ConsumerWithLocalProvider(this WebApplicationBuilder builder, Action<OAuth2ProviderConfiguration> onConfigure, string publicUrl)
    {
        OAuth2ProviderConfiguration configuration = new();
        onConfigure.Invoke(configuration);

        builder.Services.AddSingleton(new OAuth2ConsumerConfiguration()
        {
            AccessEndpoint = publicUrl + "/api/_auth/oauth2/access",
            RefreshEndpoint = publicUrl + "/api/_auth/oauth2/refresh",
            AuthorizationEndpoint = publicUrl + "/api/_auth/oauth2/authorize",
            AuthorizationRedirect = configuration.AuthorizationRedirect,
            ClientId = configuration.ClientId,
            ClientSecret = configuration.ClientSecret,
            
            ProcessComplete = (_, _) => Task.FromResult<Dictionary<string, object>>(new())
        });
        builder.Services.AddSingleton<OAuth2ConsumerService>();
        
        builder.Services.AddSingleton(configuration);
        builder.Services.AddSingleton<OAuth2ProviderService>();
    }
    
    public static void AddOAuth2Provider(this WebApplicationBuilder builder, Action<OAuth2ProviderConfiguration> onConfigure)
    {
        builder.Services.AddSingleton(sp =>
        {
            var config = new OAuth2ProviderConfiguration();
            onConfigure.Invoke(config);

            return new OAuth2ProviderService(config);
        });
    }
}