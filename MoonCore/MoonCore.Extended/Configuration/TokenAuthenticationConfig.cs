using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace MoonCore.Extended.Configuration;

public class TokenAuthenticationConfig
{
    public string AccessSecret { get; set; }
    public string RefreshSecret { get; set; }

    public int AccessDuration { get; set; } = 60;
    public int RefreshDuration { get; set; } = 3600;

    public Func<Dictionary<string, JsonElement>, Dictionary<string, object>, IServiceProvider, Task<bool>> ProcessRefresh { get; set; }
    public Func<Dictionary<string, JsonElement>, IServiceProvider, HttpContext, Task<bool>> ProcessAccess { get; set; }
}