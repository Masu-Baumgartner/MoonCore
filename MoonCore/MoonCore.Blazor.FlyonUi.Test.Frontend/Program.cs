using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MoonCore.Blazor.FlyonUi;
using MoonCore.Blazor.FlyonUi.Ace;
using MoonCore.Blazor.FlyonUi.Alerts;
using MoonCore.Blazor.FlyonUi.Auth;
using MoonCore.Blazor.FlyonUi.Drawers;
using MoonCore.Blazor.FlyonUi.Exceptions;
using MoonCore.Blazor.FlyonUi.Files;
using MoonCore.Blazor.FlyonUi.Helpers;
using MoonCore.Blazor.FlyonUi.Modals;
using MoonCore.Blazor.FlyonUi.Test.Frontend;
using MoonCore.Blazor.FlyonUi.Test.Frontend.UI;
using MoonCore.Blazor.FlyonUi.Toasts;
using MoonCore.Helpers;
using MoonCore.Logging;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Logging.ClearProviders();
builder.Logging.AddAnsiConsole();

Console.WriteLine(builder.HostEnvironment.BaseAddress);

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped(sp => new HttpApiClient(sp.GetRequiredService<HttpClient>()));

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<ModalService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<CodeEditorService>();
builder.Services.AddScoped<DropHandlerService>();
builder.Services.AddScoped<DownloadService>();
builder.Services.AddScoped<GlobalErrorService>();
builder.Services.AddScoped<DrawerService>();

builder.Services.AddFileManagerOperations();

builder.Services.AddScoped<AuthenticationStateProvider, CoolAuthStateProvider>();

await builder.Build().RunAsync();