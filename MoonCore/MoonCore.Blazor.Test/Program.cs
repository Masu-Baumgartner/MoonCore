using Microsoft.EntityFrameworkCore;
using MoonCore.Abstractions;
using MoonCore.Blazor.Extensions;
using MoonCore.Blazor.Helpers;
using MoonCore.Blazor.Test;
using MoonCore.Blazor.Test.Data;
using MoonCore.Blazor.Test.Database;
using MoonCore.Blazor.Test.Services;
using MoonCore.Blazor.Test.Shared;
using MoonCore.Extensions;
using MoonCore.Helpers;

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddMoonCore());

var dc = new SomeContext();
await DatabaseCheckHelper.Check(factory.CreateLogger<DbContext>(), dc);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddDbContext<SomeContext>();
builder.Services.AddScoped(typeof(Repository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<DevelopmentConsoleService>();

builder.Logging.AddMoonCore();

builder.Services.AddMoonCore(configuration =>
{
    configuration.Identity.Token = "30377629119932232086550615143349";
    configuration.Identity.Provider = new CustomAuthStateProvider();
    configuration.Identity.EnablePeriodicReAuth = true;
    configuration.Identity.PeriodicReAuthDelay = TimeSpan.FromSeconds(1);
});

builder.Services.AddMoonCoreBlazor(configuration =>
{
    configuration.ErrorHandler.ErrorMessageComponent = value =>
        ComponentHelper.FromType<ErrorMsgBox>(parameters => parameters.Add("Message", value));

    configuration.ErrorHandler.StacktraceComponent = value => ComponentHelper.FromType<StackTraceError>(parameters =>
    {
        parameters.Add("Content", value);
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();