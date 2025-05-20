using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MoonCore.Blazor.Tailwind.Auth;
using MoonCore.Blazor.Tailwind.Test.Models;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Test;

public class CoolestAuthStateManager : AuthenticationStateManager
{
    private readonly ILogger<CoolestAuthStateManager> Logger;
    private readonly NavigationManager Navigation;

    private ClaimsPrincipal? Identity;

    public string AccessToken { get; private set; } = "unset";

    public CoolestAuthStateManager(ILogger<CoolestAuthStateManager> logger, NavigationManager navigation)
    {
        Logger = logger;
        Navigation = navigation;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Logger.LogInformation("Loaded state");
        
        await Task.Delay(100);
        
        return GetState();
    }

    private AuthenticationState GetState()
    {
        if (Identity == null)
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        return new AuthenticationState(
            Identity
        );
    }

    public override async Task HandleLogin()
    {
        Logger.LogInformation("Login");

        if (Navigation.Uri.Contains("?auth="))
        {
            if (Navigation.Uri.Contains("?auth=user"))
            {
                Identity = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
                    {
                        new("Name", "MyCoolName"),
                        new("permissions", "only.unprivileged.*")
                    },
                    "myCoolAuth"
                ));
            }
            else if (Navigation.Uri.Contains("?auth=admin"))
            {
                Identity = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
                    {
                        new("Name", "MyCoolName"),
                        new("permissions", "privileged.*"),
                    },
                    "myCoolAuth"
                ));
            }
            else
            {
                Identity = null;
            }
            
            NotifyAuthenticationStateChanged(
                Task.FromResult(GetState())
            );
            
            Navigation.NavigateTo("/");
        }
        else
        {
            Navigation.NavigateTo("/authDemo", true);
        }
    }

    public override Task Logout()
    {
        Logger.LogInformation("Logout");

        Identity = null;
        
        NotifyAuthenticationStateChanged(
            Task.FromResult(GetState())
        );
        
        return Task.CompletedTask;
    }
}