using Microsoft.AspNetCore.Authentication;

namespace MoonCore.Blazor.FlyonUi.Test.Backend.LocalAuth;

public class LocalAuthOptions : AuthenticationSchemeOptions
{
    public string? SignInScheme { get; set; }
}