using System.Text;
using Microsoft.IdentityModel.Tokens;
using MoonCore.Blazor.FlyonUi.Test.Backend;
using MoonCore.Blazor.FlyonUi.Test.Backend.Configuration;
using MoonCore.Blazor.FlyonUi.Test.Backend.LocalAuth;
using MoonCore.Yaml;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

Directory.CreateDirectory("storage");

await YamlDefaultGenerator.GenerateAsync<AppConfiguration>(
    Path.Combine("storage", "app.yml")
);

builder.Configuration.AddYamlFile(Path.Combine("storage", "app.yml"), prefix: "MoonCoreApp");

var configuration = new AppConfiguration();

builder.Configuration
    .GetSection("MoonCoreApp")
    .Bind(configuration);

builder.Services.AddSingleton(configuration);

builder.Services.AddControllers();

builder.Services.AddAuthentication(options => { options.DefaultScheme = "Main"; })
    .AddPolicyScheme("Main", null, options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                return "Session";
            
            var auth = authHeader.FirstOrDefault();
            
            if (string.IsNullOrEmpty(auth) || !auth.StartsWith("Bearer "))
                return "Session";

            return "ApiKey";
        };
    })
    .AddJwtBearer("ApiKey", null, options =>
    {
        options.TokenValidationParameters = new()
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.Secret)
                ),
            ValidIssuer = "mooncore",
            ValidAudience = "mooncore"
        };

        options.ForwardSignIn = "Session";
        options.ForwardSignOut = "Session";
    })
    .AddCookie("Session", options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(10);

        // As redirects won't work in our spa which uses API calls
        // we need to customize the responses when certain actions happen
        options.Events.OnRedirectToLogin = async context =>
        {
            await Results.Problem(
                    title: "Unauthenticated",
                    detail: "You need to authenticate yourself to use this endpoint",
                    statusCode: 401
                )
                .ExecuteAsync(context.HttpContext);
        };

        options.Events.OnRedirectToAccessDenied = async context =>
        {
            await Results.Problem(
                    title: "Permission denied",
                    detail: "You are missing the required permissions to access this endpoint",
                    statusCode: 403
                )
                .ExecuteAsync(context.HttpContext);
        };

        options.Cookie = new CookieBuilder()
        {
            Name = "session",
            Path = "/"
        };
    })
    .AddDiscord("Discord", options =>
    {
        options.ForwardAuthenticate = "Session";
        options.ForwardSignIn = "Session";
        options.ForwardSignOut = "Session";
        
        options.SignInScheme = "Session";

        options.CallbackPath = "/api/auth.discord";
        options.AccessDeniedPath = "/";

        options.Prompt = "consent";
        
        options.ClientId = configuration.Authentication.ClientId;
        options.ClientSecret = configuration.Authentication.ClientSecret;
        
        options.Scope.Add("identify");
        options.Scope.Add("email");
    })
    .AddGitHub("Github", options =>
    {
        options.ForwardAuthenticate = "Session";
        options.ForwardSignIn = "Session";
        options.ForwardSignOut = "Session";
        
        options.SignInScheme = "Session";

        options.CallbackPath = "/api/auth.github";
        options.AccessDeniedPath = "/";
        
        options.ClientId = configuration.AuthenticationGh.ClientId;
        options.ClientSecret = configuration.AuthenticationGh.ClientSecret;
        
        options.Scope.Add("read:user");
        options.Scope.Add("user:email");
    })
    .AddScheme<LocalAuthOptions, LocalAuthHandler>("LocalAuth", "Local Authentication", options =>
    {
        options.ForwardAuthenticate = "Session";
        options.ForwardSignIn = "Session";
        options.ForwardSignOut = "Session";
        
        options.SignInScheme = "Session";
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<DemoRepository>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseWebAssemblyDebugging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();