using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using MoonCore.Blazor.Tailwind.Auth;
using MoonCore.Blazor.Tailwind.Extensions;
using MoonCore.Blazor.Tailwind.Forms;
using MoonCore.Blazor.Tailwind.Forms.Components;
using MoonCore.Blazor.Tailwind.Test;
using MoonCore.Blazor.Tailwind.Test.Extensions;
using MoonCore.Blazor.Tailwind.Test.Http.Middleware;
using MoonCore.Blazor.Tailwind.Test.Models;
using MoonCore.Blazor.Tailwind.Test.UI;
using MoonCore.Blazor.Tailwind.Test.UI.Ace;
using MoonCore.Extensions;
using MoonCore.Helpers;

FormComponentRepository.Set<int, IntComponent>();
FormComponentRepository.Set<string, StringComponent>();
FormComponentRepository.Set<bool, SwitchComponent>();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options => { options.DetailedErrors = true; })
    .AddHubOptions(options => { options.MaximumReceiveMessageSize = ByteConverter.FromMegaBytes(100).Bytes; });

builder.Logging.ClearProviders();
builder.Logging.AddMoonCore();

builder.Services.AddMoonCoreBlazorTailwind();

builder.Services.AddScoped<CodeEditorService>();

builder.Services.AddScoped(sp =>
{
    var httpApiClient = new HttpApiClient(new HttpClient(new HttpClientHandler()
    {
        Proxy = null,
        UseProxy = false
    })
    {
        BaseAddress = new Uri("http://localhost:5230/")
    });

    httpApiClient.OnConfigureRequest += message =>
    {
        var authStateManager = sp.GetRequiredService<CoolestAuthStateManager>();

        if (!string.IsNullOrEmpty(authStateManager.AccessToken))
        {
            message.Headers.Add("Authorization", $"Bearer {authStateManager.AccessToken}");
        }

        return Task.CompletedTask;
    };

    return httpApiClient;
});

builder.Services.AddAuthenticationStateManager<CoolestAuthStateManager>();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

Dictionary<int, DateTime> expireTimes = new();
expireTimes.Add(1, DateTime.MinValue);

Task.Run(async () =>
{
    await Task.Delay(TimeSpan.FromSeconds(15));
    expireTimes[1] = DateTime.UtcNow;
    
    Console.WriteLine("Invalidated");
});

builder.Services.AddSingleton(new JwtInvalidateOptions()
{
    InvalidateTimeProvider = async (_, principal) =>
    {
        var userIdClaim = principal.Claims.FirstOrDefault(x => x.Type == "userId");
        
        if (userIdClaim == null)
            return null;

        if (!int.TryParse(userIdClaim.Value, out var userId))
            return DateTime.UtcNow;

        return expireTimes[userId];
    }
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("j4epTQEhvkZJqQ9Ek88385Kr2DBKcWabcdefgh")),
        ValidIssuer = "localhost:5230",
        ValidAudience = "localhost:5230",
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();

app.UseMiddleware<JwtInvalidateMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();