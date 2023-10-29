using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Dorbit.Attributes;
using Dorbit.Extensions;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Enums;
using Dorbit.Identity.Models.Tokens;
using Dorbit.Identity.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class TokenService
{
    private readonly TokenRepository _tokenRepository;

    public TokenService(TokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    public Token Add(TokenAddRequest request)
    {
        return _tokenRepository.Insert(request.MapTo<Token>());
    }

    public TokenNewResponse New(TokenNewRequest request)
    {
        var handler = new JwtSecurityTokenHandler();
        var secret = Encoding.UTF8.GetBytes(AppSetting.Current.Security.Secret);
        var credential = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature,
            SecurityAlgorithms.Sha512Digest);
        //create jwt
        var tokenId = Guid.NewGuid();
        var token = handler.CreateToken(new SecurityTokenDescriptor()
        {
            Issuer = AppSetting.Current.Security.Issuer,
            Audience = AppSetting.Current.Security.Audience,
            Expires = DateTime.UtcNow.AddSeconds(AppSetting.Current.Security.TimeoutInSecond),
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", tokenId.ToString()),
                new Claim("UserId", request.UserId.ToString()),
                new Claim("UserId", string.Join(',', request.Accesses)),
            }),
            SigningCredentials = credential
        });

        _tokenRepository.Insert(request.MapTo(new Token()
        {
            Id = tokenId,
            State = TokenState.Valid
        }));
        
        var tokenString = handler.WriteToken(token);

        return new TokenNewResponse()
        {
            Key = tokenString
        };
    }
}