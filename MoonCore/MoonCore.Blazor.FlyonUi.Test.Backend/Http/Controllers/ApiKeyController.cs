using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoonCore.Blazor.FlyonUi.Test.Backend.Configuration;

namespace MoonCore.Blazor.FlyonUi.Test.Backend.Http.Controllers;

[ApiController]
[Route("api/apikey")]
public class ApiKeyController : Controller
{
    private readonly AppConfiguration Configuration;

    public ApiKeyController(AppConfiguration configuration)
    {
        Configuration = configuration;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<string> Get()
    {
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Audience = "mooncore",
            Issuer = "mooncore",
            Expires = DateTime.UtcNow.AddHours(1),
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow.AddMinutes(-1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration.Secret)
                ),
                SecurityAlgorithms.HmacSha256
            ),
            Claims = new Dictionary<string, object>()
            {
                { ClaimTypes.NameIdentifier, "69" },
                { ClaimTypes.Name, "API Key" },
                { ClaimTypes.Email, "bogus@email.api" }
            }
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}