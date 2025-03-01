using System.Security.Claims;

namespace MoonCore.Extended.JwtInvalidation;

public interface IJwtInvalidateOptions
{
    public Func<IServiceProvider, ClaimsPrincipal, Task<DateTime?>> InvalidateTimeProvider { get; }
    public string Scheme { get; }
}