using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using MoonCore.Blazor.Tailwind.Auth;
using MoonCore.Blazor.Tailwind.Extensions;
using MoonCore.Blazor.Tailwind.Forms;
using MoonCore.Blazor.Tailwind.Forms.Components;
using MoonCore.Blazor.Tailwind.Test;
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

builder.Services.AddScoped<CoolestAuthStateManager>();
builder.Services.AddScoped<AuthenticationStateManager>(sp => sp.GetRequiredService<CoolestAuthStateManager>());
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthenticationStateManager>());
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

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
    
    options.RefreshInterval = TimeSpan.FromSeconds(1);
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
app.UseAuthorization();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();