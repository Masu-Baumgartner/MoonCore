using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoonCore.Blazor.Tailwind.Test.Models;
using MoonCore.Exceptions;

namespace MoonCore.Blazor.Tailwind.Test.Http.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : Controller
{
    private readonly ILogger<AuthController> Logger;

    private const int AccessDuration = 3600;
    private const string SecurityKey = "j4epTQEhvkZJqQ9Ek88385Kr2DBKcWabcdefgh";

    public AuthController(ILogger<AuthController> logger)
    {
        Logger = logger;
    }

    [HttpGet("start")]
    public async Task<AuthStartModel> Start()
    {
        return new AuthStartModel()
        {
            Endpoint = "http://localhost:5230/oauth2/start",
            ClientId = "myCoolClientId",
            RedirectUri = "http://localhost:5230/"
        };
    }
    
    [HttpPost("complete")]
    public async Task<AuthCompleteModel> Complete([FromForm] string code)
    {
        // You would handle the code here
        var completePrincipal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim("Name", "Testy"),
            new Claim("userId", "1")
        ]));
        
        if (completePrincipal == null)
            throw new HttpApiException("Unable to complete authentication", 403);

        var accessData = new List<Claim>();
        accessData.AddRange(completePrincipal.Claims);
        accessData.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));

        var tokenHandler = new JwtSecurityTokenHandler();

        var std = new SecurityTokenDescriptor()
        {
            IssuedAt = DateTime.Now,
            Expires = DateTime.Now.AddSeconds(AccessDuration),
            NotBefore = DateTime.Now.AddMinutes(-1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey)),
                SecurityAlgorithms.HmacSha256
            ),
            Audience = "localhost:5230",
            Issuer = "localhost:5230",
            Claims = new Dictionary<string, object>()
        };

        foreach (var claim in accessData)
            std.Claims.Add(claim.Type, claim.Value);

        var token = tokenHandler.CreateToken(std);
        var accessToken = tokenHandler.WriteToken(token);

        return new AuthCompleteModel()
        {
            AccessToken = accessToken,
            ExpiresIn = AccessDuration
        };
    }
}