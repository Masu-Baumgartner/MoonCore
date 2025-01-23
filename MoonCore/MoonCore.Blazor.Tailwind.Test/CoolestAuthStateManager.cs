using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using MoonCore.Blazor.Tailwind.Auth;

namespace MoonCore.Blazor.Tailwind.Test;

public class CoolestAuthStateManager : AuthenticationStateManager
{
    private bool IsLoggedIn = false;
    private readonly ILogger<CoolestAuthStateManager> Logger;

    public CoolestAuthStateManager(ILogger<CoolestAuthStateManager> logger)
    {
        Logger = logger;
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
        
        await Task.Delay(100);

        IsLoggedIn = true;
        
        NotifyAuthenticationStateChanged(
            Task.FromResult(GetState())
        );
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