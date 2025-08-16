using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Test.Backend.Configuration;

public class AppConfiguration
{
    public AuthenticationConfig Authentication { get; set; } = new();
    public AuthenticationConfig AuthenticationGh { get; set; } = new();

    public string Secret { get; set; } = Formatter.GenerateString(32);
    
    public class AuthenticationConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}