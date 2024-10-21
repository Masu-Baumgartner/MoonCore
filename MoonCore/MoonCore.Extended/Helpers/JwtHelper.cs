using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MoonCore.Extended.Exceptions;
using MoonCore.Extended.Models;
using MoonCore.Helpers;

namespace MoonCore.Extended.Helpers;

public static class JwtHelper
{
    public static string Encode(string secret, JsonWebToken token)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        var hashAlgo = GetHash(token.Header.Algorithm, key);

        var header = EncodeHeader(token.Header);
        var payload = EncodePayload(token.Payload);

        var data = $"{header}.{payload}";

        var signature = GenerateSignature(hashAlgo, data);

        return $"{data}.{signature}";
    }

    public static bool VerifySignature(string secret, string jwt)
    {
        var parts = jwt.Split(".");

        if (parts.Length != 3)
            throw new JwtMalformedException("Invalid jwt provided");

        // Header
        var header = Internal_DecodeHeader(parts[0]);

        // Find hash algo
        var key = Encoding.UTF8.GetBytes(secret);
        var hashAlgo = GetHash(header.Algorithm, key);

        // Verify signature
        var data = $"{parts[0]}.{parts[1]}";
        var expectedSignature = GenerateSignature(hashAlgo, data);

        return expectedSignature == parts[2]; // The part[2] is the signature provided by the jwt
    }

    public static JsonWebToken Decode(string jwt)
    {
        var parts = jwt.Split(".");

        if (parts.Length != 3)
            throw new JwtMalformedException("Invalid jwt provided");

        JsonWebToken jsonWebToken = new()
        {
            // Header
            Header = Internal_DecodeHeader(parts[0]),

            // Payload
            Payload = Internal_DecodePayload(parts[1])
        };

        return jsonWebToken;
    }

    #region Helpers

    private static string GenerateSignature(HMAC hashAlgo, string data)
    {
        var dataBytes = Encoding.UTF8.GetBytes(data);
        var signatureBytes = hashAlgo.ComputeHash(dataBytes);

        return Formatter.FromByteToBase64(signatureBytes);
    }

    private static HMAC GetHash(string identifier, byte[] key)
    {
        switch (identifier)
        {
            case "HS512":
                return new HMACSHA512(key);
        }

        throw new JwtMalformedException($"Unsupported hash algorithm '{identifier}'");
    }

    #endregion

    #region Decoding

    private static JsonWebToken.JwtHeader Internal_DecodeHeader(string header)
    {
        try
        {
            var headerModel = JsonSerializer.Deserialize<JsonWebToken.JwtHeader?>(
                Formatter.FromBase64ToText(header)
            );

            if (!headerModel.HasValue)
                throw new JwtEncodingException("An unknown error occured while decoding header");

            return headerModel.Value;
        }
        catch (Exception e)
        {
            throw new JwtEncodingException("An error occured while decoding header", e);
        }
    }

    private static Dictionary<string, JsonElement> Internal_DecodePayload(string payload)
    {
        try
        {
            var payloadModel = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                Formatter.FromBase64ToText(payload)
            );

            if (payloadModel == null)
                throw new JwtEncodingException("An unknown error occured while decoding payload");

            return payloadModel;
        }
        catch (Exception e)
        {
            throw new JwtEncodingException("An error occured while decoding payload", e);
        }
    }

    #endregion

    #region Encoding

    private static string EncodeHeader(JsonWebToken.JwtHeader model)
    {
        try
        {
            return Formatter.FromTextToBase64(JsonSerializer.Serialize(model));
        }
        catch (Exception e)
        {
            throw new JwtEncodingException("An error occured while encoding header", e);
        }
    }

    private static string EncodePayload(Dictionary<string, JsonElement> payload)
    {
        try
        {
            return Formatter.FromTextToBase64(JsonSerializer.Serialize(payload));
        }
        catch (Exception e)
        {
            throw new JwtEncodingException("An error occured while encoding payload", e);
        }
    }

    #endregion

    #region Overloads

    public static string Encode(string secret, Action<Dictionary<string, object>> onConfigureData)
    {
        var data = new Dictionary<string, object>();
        onConfigureData.Invoke(data);

        var jwt = new JsonWebToken();
        jwt.ApplyPayload(data);
        
        return Encode(secret, jwt);
    }

    public static string Encode(string secret, Action<JsonWebToken> onConfigureToken)
    {
        var jwt = new JsonWebToken();
        onConfigureToken.Invoke(jwt);

        return Encode(secret, jwt);
    }

    public static string Encode(string secret, Action<Dictionary<string, object>> onConfigureData,
        TimeSpan validDuration)
    {
        var data = new Dictionary<string, object>();
        onConfigureData.Invoke(data);
        
        return Encode(secret, data, validDuration);
    }
    
    public static string Encode(string secret, Dictionary<string, object> data,
        TimeSpan validDuration)
    {
        var jwt = new JsonWebToken
        {
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow.AddSeconds(-1),
            ExpireTime = DateTime.UtcNow.Add(validDuration)
        };

        jwt.ApplyPayload(data);
        
        return Encode(secret, jwt);
    }

    public static bool Verify(string secret, string jwt)
    {
        if (!VerifySignature(secret, jwt))
            return false;

        var decoded = Decode(jwt);

        return decoded.AreTimestampClaimsValid;
    }

    public static bool TryVerifyAndDecode(string secret, string jwt, out JsonWebToken jsonWebToken)
    {
        try
        {
            if (!VerifySignature(secret, jwt))
            {
                jsonWebToken = null!;
                return false;
            }

            var decoded = Decode(jwt);

            if (!decoded.AreTimestampClaimsValid)
            {
                jsonWebToken = null!;
                return false;
            }

            jsonWebToken = decoded;
            return true;
        }
        catch (Exception)
        {
            jsonWebToken = null!;
            return false;
        }
    }

    public static bool TryVerifyAndDecodePayload(string secret, string jwt, out Dictionary<string, JsonElement> payload)
    {
        var isValid = TryVerifyAndDecode(secret, jwt, out var jsonWebToken);

        if (!isValid)
        {
            payload = null!;
            return false;
        }

        payload = jsonWebToken.Payload;
        return true;
    }

    public static bool TryVerify(string secret, string jwt)
    {
        try
        {
            return Verify(secret, jwt);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static Dictionary<string, JsonElement> DecodePayload(string jwt) => Decode(jwt).Payload;

    #endregion
}