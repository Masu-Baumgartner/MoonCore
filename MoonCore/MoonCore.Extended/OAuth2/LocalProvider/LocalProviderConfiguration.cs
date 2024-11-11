using MoonCore.Helpers;

namespace MoonCore.Extended.OAuth2.LocalProvider;

public class LocalProviderConfiguration
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string RedirectUri { get; set; }
    public bool AllowRegister { get; set; } = true;
    public string AccessSecret { get; set; }
    public string RefreshSecret { get; set; }
    public string PublicUrl { get; set; }
    
    // This can be generated at runtime as the tokens generated using this secret won't last long anyway
    public string CodeSecret { get; set; } = Formatter.GenerateString(32);
    public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan RefreshDuration { get; set; } = TimeSpan.FromDays(10);
}