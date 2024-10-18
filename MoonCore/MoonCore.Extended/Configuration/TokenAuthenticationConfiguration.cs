using Microsoft.AspNetCore.Http;

namespace MoonCore.Extended.Configuration;

public class TokenAuthenticationConfiguration
{
    public string AccessSecret { get; set; }
    public Func<Dictionary<string, string>, IServiceProvider, HttpContext, Task<bool>> DataLoader { get; set; }
}