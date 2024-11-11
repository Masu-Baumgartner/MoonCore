using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MoonCore.Extended.OAuth2.Consumer.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddOAuth2Authentication<T>(
        this WebApplicationBuilder builder,
        Action<AuthenticationConfiguration<T>> onConfigure
    ) where T : IUserModel
    {
        var configuration = new AuthenticationConfiguration<T>();
        onConfigure.Invoke(configuration);

        builder.Services.AddSingleton(configuration);
        builder.Services.AddSingleton<ConsumerService<T>>();
    }
}