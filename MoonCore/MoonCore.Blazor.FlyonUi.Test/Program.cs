using System.Text;
using Microsoft.IdentityModel.Tokens;
using MoonCore.Blazor.FlyonUi;
using MoonCore.Blazor.FlyonUi.Ace;
using MoonCore.Blazor.FlyonUi.Alerts;
using MoonCore.Blazor.FlyonUi.Test.UI;
using MoonCore.Blazor.FlyonUi.Auth;
using MoonCore.Blazor.FlyonUi.Files;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Files.Manager.Operations;
using MoonCore.Blazor.FlyonUi.Helpers;
using MoonCore.Blazor.FlyonUi.Modals;
using MoonCore.Blazor.FlyonUi.Test;
using MoonCore.Blazor.FlyonUi.Test.Services;
using MoonCore.Blazor.FlyonUi.Toasts;
using MoonCore.Extended.JwtInvalidation;
using MoonCore.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Logging.ClearProviders();
builder.Logging.AddAnsiConsole();

builder.Services.AddAuthenticationStateManager<CoolestAuthStateManager>();

builder.Services.AddScoped<ModalService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<CodeEditorService>();
builder.Services.AddScoped<ChunkedFileTransferService>();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<DropHandlerService>();
builder.Services.AddScoped<DownloadService>();

builder.Services.AddFileManagerOperations();

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("j4epTQEhvkZJqQ9Ek88385Kr2DBKcWabcdefgh")),
            ValidIssuer = "localhost:5220",
            ValidAudience = "localhost:5220",
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddJwtBearerInvalidation();

builder.Services.AddSingleton<IJwtInvalidateHandler, InvalidateHandler>();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

Task.Run(async () =>
{
    await Task.Delay(TimeSpan.FromSeconds(40));
    
    Console.WriteLine("Invalidated");
    InvalidateHandler.ExpireTime = DateTimeOffset.UtcNow;
});

app.Run();