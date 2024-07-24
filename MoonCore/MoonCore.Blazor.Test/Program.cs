using Microsoft.EntityFrameworkCore;
using MoonCore.Blazor.Bootstrap.Extensions;
using MoonCore.Blazor.Helpers;
using MoonCore.Blazor.Test;
using MoonCore.Blazor.Test.Data;
using MoonCore.Blazor.Test.Database;
using MoonCore.Blazor.Test.Services;
using MoonCore.Blazor.Test.Shared;
using MoonCore.Extended.Abstractions;
using MoonCore.Extended.Helpers;
using MoonCore.Extensions;

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddMoonCore());

var dc = new SomeContext();
var dbHelper = new DatabaseHelper(factory.CreateLogger<DatabaseHelper>());

dbHelper.AddDbContext<SomeContext>();
dbHelper.GenerateMappings();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddDbContext<SomeContext>();
builder.Services.AddScoped(typeof(Repository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<DevelopmentConsoleService>();

builder.Logging.AddMoonCore();

builder.Services.AddMoonCoreBlazorBootstrap(configuration =>
{
    configuration.ErrorHandler.ErrorMessageComponent = value =>
        ComponentHelper.FromType<ErrorMsgBox>(parameters => parameters.Add("Message", value));

    configuration.ErrorHandler.StacktraceComponent = value => ComponentHelper.FromType<StackTraceError>(parameters =>
    {
        parameters.Add("Content", value);
    });
});

builder.Services.AutoAddServices<Program>();


var app = builder.Build();

await dbHelper.EnsureMigrated(app.Services.CreateScope().ServiceProvider);

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