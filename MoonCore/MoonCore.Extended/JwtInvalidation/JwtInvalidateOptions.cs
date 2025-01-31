using System.Security.Claims;

namespace MoonCore.Extended.JwtInvalidation;

public class JwtInvalidateOptions
{
    public Func<IServiceProvider, ClaimsPrincipal, Task<DateTime?>> InvalidateTimeProvider { get; set; }
}