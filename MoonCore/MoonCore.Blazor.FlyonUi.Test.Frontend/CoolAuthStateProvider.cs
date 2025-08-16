using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using MoonCore.Exceptions;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Test.Frontend;

public class CoolAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpApiClient ApiClient;

    public CoolAuthStateProvider(HttpApiClient apiClient)
    {
        ApiClient = apiClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsPrincipal principal;

        try
        {
            var claims = await ApiClient.GetJson<Dictionary<string, string>>(
                "api/auth/check"
            );

            principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    claims.Select(x => new Claim(x.Key, x.Value)),
                    "RemoteAuthentication"
                )
            );
        }
        catch (HttpApiException e)
        {
            if (e.Status != 401 && e.Status != 403)
                throw;

            principal = new ClaimsPrincipal();
        }

        return new AuthenticationState(principal);
    }
}