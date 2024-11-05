using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Extended.Configuration;
using MoonCore.Extended.OAuth2.Consumer;

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
        builder.Services.AddScoped<OAuth2ConsumerService>();
    }
}