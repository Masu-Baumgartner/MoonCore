using MoonCore.Exceptions;
using MoonCore.Extended.Configuration;
using MoonCore.Http.Requests.TokenAuthentication;
using MoonCore.Http.Responses.TokenAuthentication;

namespace MoonCore.Extended.Helpers;

public static class AuthControllerHelper
{
    public static Task<RefreshResponse> Refresh(RefreshRequest request, TokenAuthenticationConfig configuration, IServiceProvider serviceProvider)
    {
        var tokenPair = TokenHelper.RefreshPair(
            request.RefreshToken,
            configuration.AccessSecret,
            configuration.RefreshSecret,
            (refreshData, newData)
                => configuration.ProcessRefresh.Invoke(refreshData, newData, serviceProvider).Result,
            configuration.AccessDuration,
            configuration.RefreshDuration
        );

        // Handle refresh error
        if (!tokenPair.HasValue)
            throw new HttpApiException("Unable to refresh token", 401);

        // Return data
        return Task.FromResult(new RefreshResponse()
        {
            AccessToken = tokenPair.Value.AccessToken,
            RefreshToken = tokenPair.Value.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddSeconds(configuration.AccessDuration)
        });
    }
}