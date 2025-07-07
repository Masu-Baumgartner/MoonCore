using System.Security.Claims;

namespace MoonCore.Extended.JwtInvalidation;

public interface IJwtInvalidateHandler
{
    public Task<bool> Handle(ClaimsPrincipal principal);
}