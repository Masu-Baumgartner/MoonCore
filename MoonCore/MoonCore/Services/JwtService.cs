using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using MoonCore.Helpers;
using Newtonsoft.Json;

namespace MoonCore.Services;

public class JwtService<T> where T : struct, Enum
{
    private readonly string Token;
    
    public JwtService(string token)
    {
        Token = token;
    }
    
    public Task<string> Create(Action<Dictionary<string, string>> data, T type, TimeSpan validDuration)
    {
        var builder = new JwtBuilder()
            .WithSecret(Token)
            .IssuedAt(DateTime.UtcNow)
            .AddHeader("type", type)
            .ExpirationTime(DateTime.UtcNow.Add(validDuration))
            .WithAlgorithm(new HMACSHA512Algorithm());

        var dataDic = new Dictionary<string, string>();
        data.Invoke(dataDic);

        foreach (var entry in dataDic)
            builder = builder.AddClaim(entry.Key, entry.Value);

        var jwt = builder.Encode();
        
        return Task.FromResult(jwt);
    }

    public Task<bool> Validate(string token, params T[] allowedTypes)
    {
        // Null/empty check
        if (string.IsNullOrEmpty(token))
            return Task.FromResult(false);
        
        try
        {
            // Without the body decode call the jwt validation would not work for some weird reason.
            // It would not throw an error when the signature is invalid
            _ = new JwtBuilder()
                .WithSecret(Token)
                .WithAlgorithm(new HMACSHA512Algorithm())
                .MustVerifySignature()
                .Decode(token);
            
            var headerJson = new JwtBuilder()
                .WithSecret(Token)
                .WithAlgorithm(new HMACSHA512Algorithm())
                .MustVerifySignature()
                .DecodeHeader(token);

            if (headerJson == null)
                return Task.FromResult(false);

            // Jwt type validation
            if (allowedTypes.Length == 0)
                return Task.FromResult(true);

            var headerData = JsonConvert.DeserializeObject<Dictionary<string, string>>(headerJson);

            if (headerData == null) // => Invalid header
                return Task.FromResult(false);

            if (!headerData.ContainsKey("type")) // => Invalid header, Type is missing
                return Task.FromResult(false);

            foreach (var name in allowedTypes)
            {
                if (headerData["type"] == name.ToString()) // => Correct type found
                    return Task.FromResult(true);
            }

            // None found? Invalid type!
            return Task.FromResult(false);
        }
        catch (SignatureVerificationException)
        {
            Logger.Warn($"A manipulated jwt has been found. Required jwt types: {string.Join(" ", Enum.GetNames<T>())} Jwt: {token}");
            
            return Task.FromResult(false);
        }
        catch (Exception e)
        {
            Logger.Warn(e);
            
            return Task.FromResult(false);
        }
    }

    public Task<Dictionary<string, string>> Decode(string token)
    {
        try
        {
            var json = new JwtBuilder()
                .WithSecret(Token)
                .WithAlgorithm(new HMACSHA512Algorithm())
                .MustVerifySignature()
                .Decode(token);

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            return Task.FromResult(data)!;
        }
        catch (SignatureVerificationException)
        {
            return Task.FromResult(new Dictionary<string, string>());
        }
        catch (Exception e)
        {
            Logger.Warn("An unknown error occured while processing token");
            Logger.Warn(e);
            
            return Task.FromResult<Dictionary<string, string>>(null!);
        }
    }
}