using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace MoonCore.Extended.Configuration;

public class TokenAuthenticationConfiguration
{
    public string AccessSecret { get; set; }
    public Func<Dictionary<string, JsonElement>, IServiceProvider, HttpContext, Task<bool>> DataLoader { get; set; }
}