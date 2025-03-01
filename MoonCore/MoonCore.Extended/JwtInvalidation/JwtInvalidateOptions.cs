using System.Security.Claims;

namespace MoonCore.Extended.JwtInvalidation;

public class JwtInvalidateOptions : IJwtInvalidateOptions
{
    public Func<IServiceProvider, ClaimsPrincipal, Task<DateTime?>> InvalidateTimeProvider { get; set; }
    public string Scheme { get; set; } = "Bearer";
}