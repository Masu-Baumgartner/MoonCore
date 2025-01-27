using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoonCore.Blazor.Tailwind.Test.Models;

namespace MoonCore.Blazor.Tailwind.Test.Http.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : Controller
{
    private readonly ILogger<AuthController> Logger;

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
            RedirectUri = "http://localhost:5230/#auth"
        };
    }

    [HttpPost("refresh")]
    public async Task<AuthCompleteModel> Refresh([FromBody] AuthRefreshModel model)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var x = tokenHandler.ValidateToken(model.RefreshToken, new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("j4epTQEhvkZJqQ9Ek88385Kr2DBKcWabcdefgh")),
                ValidIssuer = "localhost:5230",
                ValidAudience = "localhost:5230",
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            if (x == null)
                Logger.LogInformation("Null");
            else
            {
                foreach (var claim in x.Claims)
                {
                    Logger.LogInformation("{type}: {value}", claim.Type, claim.Value);
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError("Verification: {e}", e);
        }
        
        return new();
        /*
        var accessTokenJwt = new JwtSecurityToken(
            claims:
            [
                new Claim("Name", "CoolUser")
            ],
            issuer: "localhost:5230",
            audience: "localhost:5230",
            notBefore: DateTime.Now.AddMinutes(-1),
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("j4epTQEhvkZJqQ9Ek88385Kr2DBKcWabcdefgh")),
                SecurityAlgorithms.HmacSha256
            )
        );
        
        var refreshTokenJwt = new JwtSecurityToken(
            claims:
            [
                new Claim("Name", "CoolUser")
            ],
            issuer: "localhost:5230",
            audience: "localhost:5230",
            notBefore: DateTime.Now.AddMinutes(-1),
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("j4epTQEhvkZJqQ9Ek88385Kr2DBKcWabcdefgh")),
                SecurityAlgorithms.HmacSha256
            )
        );

        var accessToken = tokenHandler.WriteToken(accessTokenJwt);
        var refreshToken = tokenHandler.WriteToken(refreshTokenJwt);

        return new AuthCompleteModel()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 30 * 60
        };*/
    }

    [HttpPost("complete")]
    public async Task<AuthCompleteModel> Complete([FromForm] string code)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var accessTokenJwt = new JwtSecurityToken(
            claims:
            [
                new Claim("Name", "CoolUser")
            ],
            issuer: "localhost:5230",
            audience: "localhost:5230",
            notBefore: DateTime.Now.AddMinutes(-1),
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("j4epTQEhvkZJqQ9Ek88385Kr2DBKcWabcdefgh")),
                SecurityAlgorithms.HmacSha256
            )
        );
        
        var refreshTokenJwt = new JwtSecurityToken(
            claims:
            [
                new Claim("Name", "CoolUser")
            ],
            issuer: "localhost:5230",
            audience: "localhost:5230",
            notBefore: DateTime.Now.AddMinutes(-1),
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("j4epTQEhvkZJqQ9Ek88385Kr2DBKcWabcdefgh")),
                SecurityAlgorithms.HmacSha256
            )
        );

        var accessToken = tokenHandler.WriteToken(accessTokenJwt);
        var refreshToken = tokenHandler.WriteToken(refreshTokenJwt);

        return new AuthCompleteModel()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 30 * 60
        };
    }
}