using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MoonCore.Extended.Helpers;

public class JwtHelper
{
    private readonly ILogger<JwtHelper> Logger;

    public JwtHelper(ILogger<JwtHelper> logger)
    {
        Logger = logger;
    }

    #region With type specification (obsolete)

    [Obsolete(
        "Use the jwt methods without the type specification and use a separate secret for every type of jwt you use")]
    public Task<string> Create(string secret, Action<Dictionary<string, string>> data, string typeIdentifier,
        TimeSpan validDuration)
    {
        var builder = new JwtBuilder()
            .WithSecret(secret)
            .IssuedAt(DateTime.UtcNow)
            .AddHeader("type", typeIdentifier)
            .ExpirationTime(DateTime.UtcNow.Add(validDuration))
            .WithAlgorithm(new HMACSHA512Algorithm());

        var dataDic = new Dictionary<string, string>();
        data.Invoke(dataDic);

        foreach (var entry in dataDic)
            builder = builder.AddClaim(entry.Key, entry.Value);

        var jwt = builder.Encode();

        return Task.FromResult(jwt);
    }

    [Obsolete(
        "Use the jwt methods without the type specification and use a separate secret for every type of jwt you use")]
    public Task<bool> Validate(string secret, string jwt, string allowedType)
    {
        // Null/empty check
        if (string.IsNullOrEmpty(jwt))
            return Task.FromResult(false);

        try
        {
            // Without the body decode call the jwt validation would not work for some weird reason.
            // It would not throw an error when the signature is invalid
            _ = new JwtBuilder()
                .WithSecret(secret)
                .WithAlgorithm(new HMACSHA512Algorithm())
                .MustVerifySignature()
                .Decode(jwt);

            var headerJson = new JwtBuilder()
                .WithSecret(secret)
                .WithAlgorithm(new HMACSHA512Algorithm())
                .MustVerifySignature()
                .DecodeHeader(jwt);

            if (headerJson == null)
                return Task.FromResult(false);

            var headerData = JsonConvert.DeserializeObject<Dictionary<string, string>>(headerJson);

            if (headerData == null) // => Invalid header
                return Task.FromResult(false);

            if (!headerData.ContainsKey("type")) // => Invalid header, Type is missing
                return Task.FromResult(false);

            if (headerData["type"] == allowedType) // => Correct type found
                return Task.FromResult(true);

            // None found? Invalid type!
            return Task.FromResult(false);
        }
        catch (TokenExpiredException)
        {
            return Task.FromResult(false);
        }
        catch (TokenNotYetValidException)
        {
            return Task.FromResult(false);
        }
        catch (SignatureVerificationException)
        {
            Logger.LogWarning("A manipulated jwt has been found.  Jwt: {jwt}", jwt);

            return Task.FromResult(false);
        }
        catch (Exception e)
        {
            Logger.LogWarning("An error occured while validating a jwt: {e}", e);

            return Task.FromResult(false);
        }
    }

    #endregion

    #region Without type specification

    public Task<string> Create(string secret, Action<Dictionary<string, string>> data, TimeSpan duration)
    {
        Dictionary<string, string> dataDic = new();

        data.Invoke(dataDic);

        return Create(secret, dataDic, duration);
    }
    
    public Task<string> Create(string secret, Dictionary<string, string> data, TimeSpan validDuration)
    {
        var builder = new JwtBuilder()
            .WithSecret(secret)
            .IssuedAt(DateTime.UtcNow)
            .ExpirationTime(DateTime.UtcNow.Add(validDuration))
            .WithAlgorithm(new HMACSHA512Algorithm());

        foreach (var entry in data)
            builder = builder.AddClaim(entry.Key, entry.Value);

        var jwt = builder.Encode();

        return Task.FromResult(jwt);
    }

    public Task<bool> Validate(string secret, string jwt)
    {
        // Null/empty check
        if (string.IsNullOrEmpty(jwt))
            return Task.FromResult(false);

        try
        {
            // Without the body decode call the jwt validation would not work for some weird reason.
            // It would not throw an error when the signature is invalid
            _ = new JwtBuilder()
                .WithSecret(secret)
                .WithAlgorithm(new HMACSHA512Algorithm())
                .MustVerifySignature()
                .Decode(jwt);

            return Task.FromResult(true);
        }
        catch (TokenExpiredException)
        {
            return Task.FromResult(false);
        }
        catch (TokenNotYetValidException)
        {
            return Task.FromResult(false);
        }
        catch (SignatureVerificationException)
        {
            Logger.LogWarning("A manipulated jwt has been found.  Jwt: {jwt}", jwt);

            return Task.FromResult(false);
        }
        catch (Exception e)
        {
            Logger.LogWarning("An error occured while validating a jwt: {e}", e);

            return Task.FromResult(false);
        }
    }

    #endregion

    [Obsolete("Consider switching to the Decode(string jwt) method as the decode method skips validation. If you want to validate a jwt use the Validate(string secret, string jwt) method")]
    public Task<Dictionary<string, string>> Decode(string secret, string jwt) => Decode(jwt);
    
    public Task<Dictionary<string, string>> Decode(string jwt)
    {
        try
        {
            var json = new JwtBuilder()
                .WithAlgorithm(new HMACSHA512Algorithm())
                .Decode(jwt);

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            return Task.FromResult(data)!;
        }
        catch (Exception e)
        {
            Logger.LogWarning("An unknown error occured while processing token: {e}", e);

            return Task.FromResult<Dictionary<string, string>>(null!);
        }
    }
}