using MoonCore.Blazor.Tailwind.Extensions;
using MoonCore.Blazor.Tailwind.Forms;
using MoonCore.Blazor.Tailwind.Forms.Components;
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
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true;
    })
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = ByteConverter.FromMegaBytes(100).Bytes;
    });

builder.Logging.ClearProviders();
builder.Logging.AddMoonCore();

builder.Services.AddMoonCoreBlazorTailwind();

builder.Services.AddScoped<CodeEditorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();