using MoonCore.Extended.Models;

namespace MoonCore.Extended.Configuration;

public class LocalOAuth2ProviderConfig
{
    public Func<int, Task<LocalOAuth2User>> LoadUserData { get; set; }
    public Func<string, string, Task<int>> HandleLogin { get; set; }
    public Func<string, string, string, Task<int>> HandleRegister { get; set; }
    public Func<int, Task<bool>> Validate { get; set; }

    public bool AllowRegister { get; set; } = true;
}