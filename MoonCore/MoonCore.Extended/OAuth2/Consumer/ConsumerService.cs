using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Exceptions;
using MoonCore.Extended.Helpers;
using MoonCore.Models;
using MoonCore.OAuth2.Responses;

namespace MoonCore.Extended.OAuth2.Consumer;

public class ConsumerService<T> where T : IUserModel
{
    private readonly AuthenticationConfiguration<T> Configuration;
    private readonly ILogger<ConsumerService<T>> Logger;

    public ConsumerService(
        AuthenticationConfiguration<T> configuration,
        ILogger<ConsumerService<T>> logger
    )
    {
        Configuration = configuration;
        Logger = logger;
    }

    public Task<StartResponse> Start()
    {
        return Task.FromResult(new StartResponse()
        {
            Endpoint = Configuration.AuthorizeEndpoint,
            ClientId = Configuration.ClientId,
            RedirectUri = Configuration.RedirectUri
        });
    }

    public async Task<T?> ValidateAccess(IServiceProvider serviceProvider, string accessToken)
    {
        var userId = -1;
        var isValidToken = TokenHelper.IsValidToken(accessToken, Configuration.AccessSecret, parameters
            => parameters.TryGetValue("userId", out var userIdEl) && userIdEl.TryGetInt32(out userId)
        );

        if (!isValidToken)
            return default;

        var dataProvider = serviceProvider.GetRequiredService<IDataProvider<T>>();
        var user = await dataProvider.LoadById(userId);

        return user;
    }

    public async Task<TokenPair> Refresh(IServiceProvider serviceProvider, string refreshToken)
    {
        var userId = -1;

        // Check if the refresh token is valid
        var isValidRefresh = TokenHelper.IsValidToken(
            refreshToken,
            Configuration.RefreshSecret,
            parameters => parameters.TryGetValue("userId", out var userIdEl) && userIdEl.TryGetInt32(out userId)
        );

        if (!isValidRefresh)
            throw new HttpApiException("Invalid or expired refresh token provided", 401);

        // Load user by the extracted id
        var dataProvider = serviceProvider.GetRequiredService<IDataProvider<T>>();
        var user = await dataProvider.LoadById(userId);

        if (user == null)
            throw new HttpApiException("Invalid user id in refresh token", 400);

        // Handle oauth2 refresh
        if (DateTime.UtcNow > user.RefreshTimestamp) // It's time to refresh our access
        {
            TokenPair refreshedPair;

            try
            {
                var oauth2Provider = serviceProvider.GetRequiredService<IOAuth2Provider<T>>();
                refreshedPair = await oauth2Provider.ProcessRefresh(user.RefreshToken);
            }
            catch (Exception
                   e) // We handle this as a soft error as this occurs everytime a user didn't log in within the configured timeframe
            {
                Logger.LogTrace("An error occured while refreshing oauth2 for user '{userId}': {e}", userId, e);
                throw new HttpApiException("Unable to refresh access from oauth2 provider", 401);
            }

            // Update the values in the data storage for later use
            user.AccessToken = refreshedPair.AccessToken;
            user.RefreshToken = refreshedPair.RefreshToken;
            user.RefreshTimestamp = DateTime.UtcNow.AddSeconds(refreshedPair.ExpiresIn);

            await dataProvider.SaveChanges(user);
        }

        return await GenerateAccess(user);
    }

    public async Task<TokenPair> Complete(IServiceProvider serviceProvider, string code)
    {
        var oauth2Provider = serviceProvider.GetRequiredService<IOAuth2Provider<T>>();
        
        // Request access resources from provider
        var accessResponse = await oauth2Provider.ProcessAccess(code);

        // and synchronize user
        var user = await oauth2Provider.ProcessSync(accessResponse.AccessToken);

        // Update the values in the data storage for later use
        user.AccessToken = accessResponse.AccessToken;
        user.RefreshToken = accessResponse.RefreshToken;
        user.RefreshTimestamp = DateTime.UtcNow.AddSeconds(accessResponse.ExpiresIn);

        var dataProvider = serviceProvider.GetRequiredService<IDataProvider<T>>();
        await dataProvider.SaveChanges(user);

        // Return new generated access
        return await GenerateAccess(user);
    }

    private Task<TokenPair> GenerateAccess(T model)
    {
        var tokenPair = TokenHelper.GeneratePair(
            Configuration.AccessSecret,
            Configuration.RefreshSecret,
            parameters => { parameters.Add("userId", model.Id); },
            (int)Configuration.RefreshInterval.TotalSeconds,
            (int)Configuration.RefreshDuration.TotalSeconds
        );

        return Task.FromResult(tokenPair);
    }
}