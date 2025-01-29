using System.Security.Claims;

namespace MoonCore.Blazor.Tailwind.Test.Models;

public class JwtInvalidateOptions
{
    public Func<IServiceProvider, ClaimsPrincipal, Task<DateTime?>> InvalidateTimeProvider { get; set; }
}