using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MoonCore.Blazor.FlyonUi.Test.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    [AllowAnonymous]
    [HttpGet("gen")]
    public async Task<string> GetToken()
    {
        var securityTokenDescriptor = new SecurityTokenDescriptor()
        {
            Expires = DateTime.Now.AddMinutes(60),
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now.AddMinutes(-1),
            Claims = new Dictionary<string, object>(),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("j4epTQEhvkZJqQ9Ek88385Kr2DBKcWabcdefgh")
                ),
                SecurityAlgorithms.HmacSha256
            ),
            Issuer = "localhost:5220",
            Audience = "localhost:5220"
        };

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);

        var jwt = jwtSecurityTokenHandler.WriteToken(securityToken);

        return jwt;
    }

    [Authorize]
    [HttpGet("check")]
    public async Task<string> Check() => "Hello World";
}