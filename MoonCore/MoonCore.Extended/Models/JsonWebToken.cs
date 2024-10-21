using System.Text.Json;
using System.Text.Json.Serialization;
using MoonCore.Extended.Exceptions;

namespace MoonCore.Extended.Models;

public class JsonWebToken
{
    public JwtHeader Header { get; set; }
    public Dictionary<string, JsonElement> Payload { get; set; }

    #region Claims

    public string Issuer
    {
        get => Payload["iss"].GetString()!;
        set => Payload["iss"] = JsonSerializer.SerializeToElement(value);
    }

    public string Subject
    {
        get => Payload["sub"].GetString()!;
        set => Payload["sub"] = JsonSerializer.SerializeToElement(value);
    }
    
    public string Audience
    {
        get => Payload["aud"].GetString()!;
        set => Payload["aud"] = JsonSerializer.SerializeToElement(value);
    }

    public string Identifier
    {
        get => Payload["jti"].GetString()!;
        set => Payload["jti"] = JsonSerializer.SerializeToElement(value);
    }
    
    public DateTime IssuedAt
    {
        get => DateTimeOffset.FromUnixTimeSeconds(Payload["iat"].GetInt64()).UtcDateTime;
        set => Payload["iat"] = JsonSerializer.SerializeToElement(((DateTimeOffset)value).ToUnixTimeSeconds());
    }
    
    public DateTime ExpireTime
    {
        get => DateTimeOffset.FromUnixTimeSeconds(Payload["exp"].GetInt64()).UtcDateTime;
        set => Payload["exp"] = JsonSerializer.SerializeToElement(((DateTimeOffset)value).ToUnixTimeSeconds());
    }
    
    public DateTime NotBefore
    {
        get => DateTimeOffset.FromUnixTimeSeconds(Payload["nbf"].GetInt64()).UtcDateTime;
        set => Payload["nbf"] = JsonSerializer.SerializeToElement(((DateTimeOffset)value).ToUnixTimeSeconds());
    }

    #endregion
    
    public bool AreTimestampClaimsValid
    {
        get
        {
            if (Payload.ContainsKey("iat") && DateTime.UtcNow < IssuedAt)
                return false;

            if (Payload.ContainsKey("nbf") && DateTime.UtcNow < NotBefore)
                return false;

            if (Payload.ContainsKey("exp") && DateTime.UtcNow > ExpireTime)
                return false;

            return true;
        }
    }

    public JsonWebToken()
    {
        Header = new()
        {
            Algorithm = "HS512",
            Type = "JWT"
        };

        Payload = new(); 
    }

    public void CheckTimestampClaims()
    {
        if (Payload.ContainsKey("iat") && DateTime.UtcNow < IssuedAt)
            throw new JwtClaimException("The issued at claim is set to a point in the future");

        if (Payload.ContainsKey("nbf") && DateTime.UtcNow < NotBefore)
            throw new JwtClaimException("The jwt is not yet valid");

        if (Payload.ContainsKey("exp") && DateTime.UtcNow > ExpireTime)
            throw new JwtClaimException("The jwt is expired");
    }

    public void ApplyPayload(Dictionary<string, object> payloadData)
    {
        foreach (var kvp in payloadData)
            Payload[kvp.Key] = JsonSerializer.SerializeToElement(kvp.Value);
    }
    
    public struct JwtHeader
    {
        [JsonPropertyName("alg")] public string Algorithm { get; set; }
        [JsonPropertyName("typ")] public string Type { get; set; }
    }
}