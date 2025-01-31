using Microsoft.AspNetCore.Components.Authorization;

namespace MoonCore.Blazor.Tailwind.Auth;

public abstract class AuthenticationStateManager : AuthenticationStateProvider
{
    public abstract Task HandleLogin();
    public abstract Task Logout();
}