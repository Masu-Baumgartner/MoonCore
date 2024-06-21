using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Abstractions;
using MoonCore.Helpers;
using MoonCore.Models;

namespace MoonCore.Services;

public class IdentityService : IDisposable
{
    public SmartEventHandler OnStateChanged { get; set; }
    public bool IsAuthenticated { get; private set; } = false;
    public string Identifier { get; private set; }
    public readonly DynamicStorage Storage = new();

    private readonly AuthenticationStateProvider Provider;
    private readonly IServiceProvider ServiceProvider;
    private readonly JwtService<AuthEnum> JwtService;
    private readonly ILogger<IdentityService> Logger;
    private readonly MoonCoreConfiguration Configuration;

    private readonly Timer ReAuthTimer;

    private string CurrentJwt = "";

    public IdentityService(
        MoonCoreConfiguration configuration,
        IServiceProvider serviceProvider,
        ILogger<IdentityService> logger
    )
    {
        var eventHandlerLogger = serviceProvider.GetRequiredService<ILogger<SmartEventHandler>>();
        var jwtLogger = serviceProvider.GetRequiredService<ILogger<JwtService<AuthEnum>>>();

        Provider = configuration.Identity.Provider;
        ServiceProvider = serviceProvider;
        Configuration = configuration;
        OnStateChanged = new(eventHandlerLogger);
        Logger = logger;

        JwtService = new(configuration.Identity.Token, jwtLogger);

        if (configuration.Identity.EnablePeriodicReAuth)
        {
            Logger.LogTrace("Starting periodic re-authentication with {delay}",
                Formatter.FormatUptime(configuration.Identity.PeriodicReAuthDelay)
            );

            ReAuthTimer = new(async _ =>
            {
                await Authenticate();
                
                Logger.LogTrace("Re-authenticated");
            }, null, TimeSpan.Zero, configuration.Identity.PeriodicReAuthDelay);
        }
    }

    public async Task<string> Login(string identifier, TimeSpan? tokenDuration = null)
    {
        var jwt = await JwtService.Create(parameters =>
        {
            parameters.Add("identifier", identifier);
            
            // Not required as the jwt service is handling it already
            //parameters.Add("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
        }, AuthEnum.MoonCoreLogin, tokenDuration ?? Configuration.Identity.DefaultTokenDuration);

        CurrentJwt = jwt;

        await Authenticate();

        return jwt;
    }

    public async Task Logout()
    {
        CurrentJwt = "";

        await Authenticate();
    }

    public async Task Authenticate(string jwt)
    {
        CurrentJwt = jwt;

        await Authenticate();
    }

    public async Task Authenticate()
    {
        var lastAuthState = IsAuthenticated;
        var lastIdentifier = Identifier;

        await Internal_Authenticate();

        // Ignore if no state has been changed
        if (lastAuthState == IsAuthenticated && lastIdentifier == Identifier)
            return;

        await OnStateChanged.Invoke();
    }

    private async Task Internal_Authenticate()
    {
        Logger.LogTrace("Clearing authentication cache");

        IsAuthenticated = false;
        Storage.Clear();

        // JWT validation
        if (!await JwtService.Validate(CurrentJwt, AuthEnum.MoonCoreLogin))
        {
            Logger.LogTrace("JWT is invalid: {jwt}", CurrentJwt);
            return;
        }

        // Decode JWT
        var data = await JwtService.Decode(CurrentJwt);

        // Validate content
        if (!data.ContainsKey("identifier") || !data.ContainsKey("iat"))
        {
            Logger.LogTrace("The jwt content is missing identifier and/or iat field");
            return;
        }

        var issuedAtString = data["iat"];
        var identifier = data["identifier"];

        // Check if identifier is valid
        if (!await Provider.IsValidIdentifier(ServiceProvider, identifier))
        {
            Logger.LogTrace("The auth provider reported the identifier {identifier} as invalid", identifier);
            return;
        }

        // Validate issued at
        if (!long.TryParse(issuedAtString, out var issuedAtLong))
        {
            Logger.LogTrace("The IssuedAt field cannot be parsed. Content: {content}", issuedAtString);
            return;
        }

        var issuedAt = DateTimeOffset.FromUnixTimeSeconds(issuedAtLong).DateTime;

        // Load issued at from provider
        var tokenValidTimestamp = await Provider.DetermineTokenValidTimestamp(ServiceProvider, identifier);

        if (tokenValidTimestamp > issuedAt)
        {
            Logger.LogTrace("The token valid timestamp is greater than the IssuedAt timestamp. The token is expired");
            return;
        }

        IsAuthenticated = true;
        Identifier = identifier;

        await Provider.LoadFromIdentifier(ServiceProvider, identifier, Storage);
    }

    public void Dispose()
    {
        ReAuthTimer?.Dispose();
    }
}