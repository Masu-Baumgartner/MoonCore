using System.Security.Claims;

namespace MoonCore.Authentication;

public class PermClaimsPrinciple : ClaimsPrincipal
{
    public string[] Permissions { get; set; }
    public object IdentityModel { get; set; }
}