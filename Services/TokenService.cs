using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Extensions;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Enums;
using Dorbit.Identity.Models.Tokens;
using Dorbit.Identity.Repositories;
using Microsoft.IdentityModel.Tokens;
using UAParser;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class TokenService
{
    private const string TokenAlgorithm = SecurityAlgorithms.HmacSha256Signature;
    private const string TokenDigest = SecurityAlgorithms.Sha512Digest;
    
    private readonly TokenRepository _tokenRepository;

    public TokenService(TokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    public Task<Token> Add(TokenAddRequest request)
    {
        return _tokenRepository.InsertAsync(request.MapTo<Token>());
    }

    private SecurityKey GetKey()
    {
        var secret = Encoding.UTF8.GetBytes(App.AppSetting.Security.Secret);
        return new SymmetricSecurityKey(secret);
    }

    public async Task<TokenResponse> CreateAsync(TokenNewRequest request)
    {
        var credential = new SigningCredentials(GetKey(), TokenAlgorithm, TokenDigest);
        //create jwt
        var tokenId = Guid.NewGuid();
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(new SecurityTokenDescriptor()
        {
            Issuer = App.AppSetting.Security.Issuer,
            Audience = App.AppSetting.Security.Audience,
            Expires = DateTime.UtcNow.ToUniversalTime().AddSeconds(App.AppSetting.Security.TimeoutInSecond),
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", tokenId.ToString()),
            }),
            SigningCredentials = credential
        });

        var uaParser = Parser.GetDefault();
        var clientInfo = uaParser.Parse(request.UserAgent ?? "");

        await _tokenRepository.InsertAsync(request.MapTo(new Token()
        {
            Id = tokenId,
            Os = clientInfo.OS.Family,
            Platform = clientInfo.Device.Family,
            Application = clientInfo.Browser.Family,
            State = TokenState.Valid
        }));

        var tokenString = handler.WriteToken(token);

        return new TokenResponse()
        {
            Key = tokenString
        };
    }

    public async Task<TokenValidationResult> ValidateAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var parameters = new TokenValidationParameters()
        {
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = App.AppSetting.Security.Issuer,
            ValidAudience = App.AppSetting.Security.Audience,
            IssuerSigningKey = GetKey()
        };
        return await tokenHandler.ValidateTokenAsync(token, parameters);
    }
}