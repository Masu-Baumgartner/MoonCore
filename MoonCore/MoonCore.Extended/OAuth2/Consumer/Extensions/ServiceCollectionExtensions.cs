using Microsoft.Extensions.DependencyInjection;

namespace MoonCore.Extended.OAuth2.Consumer.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddOAuth2Authentication<T>(this IServiceCollection collection, Action<AuthenticationConfiguration<T>> onConfigure) where T : IUserModel
    {
        var configuration = new AuthenticationConfiguration<T>();
        onConfigure.Invoke(configuration);

        collection.AddSingleton(configuration);
        collection.AddSingleton<ConsumerService<T>>();
    }
}