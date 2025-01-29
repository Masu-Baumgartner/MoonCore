using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MoonCore.Blazor.Tailwind.Auth;
using MoonCore.Blazor.Tailwind.Test.Models;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Test;

public class CoolestAuthStateManager : AuthenticationStateManager
{
    private bool IsLoggedIn = true;
    private readonly ILogger<CoolestAuthStateManager> Logger;
    private readonly NavigationManager Navigation;
    private readonly HttpApiClient HttpApiClient;

    public string AccessToken { get; private set; } = "unset";

    public CoolestAuthStateManager(ILogger<CoolestAuthStateManager> logger, NavigationManager navigation, HttpApiClient httpApiClient)
    {
        Logger = logger;
        Navigation = navigation;
        HttpApiClient = httpApiClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Logger.LogInformation("Loaded state");
        
        await Task.Delay(100);
        
        return GetState();
    }

    private AuthenticationState GetState()
    {
        if (IsLoggedIn)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
                {
                    new("Name", "MyCoolName")
                },
                "myCoolAuth"
            )));
        }
        else
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public override async Task HandleLogin()
    {
        Logger.LogInformation("Login");

        if (Navigation.Uri.Contains("?code="))
        {
            var uri = new Uri(Navigation.Uri);
            var queryParser = new QueryStringBuilder(uri.Query);
            var codeParam = queryParser["code"]!;
            
            var authCompleteData = await HttpApiClient.PostJson<AuthCompleteModel>("auth/complete", new FormUrlEncodedContent(new []
            {
                new KeyValuePair<string, string>("code", codeParam)
            }));
            
            Logger.LogInformation("Received complete data with valid access token for {seconds} seconds", authCompleteData.ExpiresIn);
            Logger.LogInformation("Access token: {accessToken}", authCompleteData.AccessToken);
            
            AccessToken = authCompleteData.AccessToken;
            IsLoggedIn = true;
        
            NotifyAuthenticationStateChanged(
                Task.FromResult(GetState())
            );
            
            Navigation.NavigateTo("/");
        }
        else
        {
            var authStartData = await HttpApiClient.GetJson<AuthStartModel>("auth/start");
            var oauth2Endpoint = authStartData.Endpoint;

            oauth2Endpoint += $"?client_id={authStartData.ClientId}" +
                              $"&redirect_uri={authStartData.RedirectUri}";
            
            Navigation.NavigateTo(oauth2Endpoint, true);
        }
    }

    public override Task Logout()
    {
        Logger.LogInformation("Logout");

        IsLoggedIn = false;
        
        NotifyAuthenticationStateChanged(
            Task.FromResult(GetState())
        );
        
        return Task.CompletedTask;
    }
}