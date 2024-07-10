using MoonCore.Abstractions;

namespace MoonCore.Models;

public class MoonCoreIdentityConfiguration
{
    public AuthenticationStateProvider Provider { get; set; }
    public string Token { get; set; } = "";
    public TimeSpan DefaultTokenDuration { get; set; } = TimeSpan.FromDays(10);
    public bool EnablePeriodicReAuth { get; set; } = false;
    public TimeSpan PeriodicReAuthDelay { get; set; } = TimeSpan.FromMinutes(15);
}