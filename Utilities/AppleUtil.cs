using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Dorbit.Identity.Utilities;

public class AppleUtil
{
    public static string GenerateClientSecret(string teamId, string clientId, string keyId, string privateKey)
    {
        var securityKey = new ECDsaSecurityKey(ECDsa.Create())
        {
            KeyId = keyId
        };
        securityKey.ECDsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey), out _);

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha256);

        var handler = new JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = teamId,
            Audience = "https://appleid.apple.com",
            Subject = new System.Security.Claims.ClaimsIdentity(new[] {
                new System.Security.Claims.Claim("sub", clientId)
            }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = credentials
        };

        var token = handler.CreateJwtSecurityToken(descriptor);
        return handler.WriteToken(token);
    }
}