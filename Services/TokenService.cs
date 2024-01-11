using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Models.Jwts;
using Dorbit.Framework.Services;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Enums;
using Dorbit.Identity.Models.Tokens;
using Dorbit.Identity.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using UAParser;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class TokenService
{
    private readonly TokenRepository _tokenRepository;
    private readonly JwtService _jwtService;
    private readonly IMemoryCache _memoryCache;

    public TokenService(TokenRepository tokenRepository, JwtService jwtService, IMemoryCache memoryCache)
    {
        _tokenRepository = tokenRepository;
        _jwtService = jwtService;
        _memoryCache = memoryCache;
    }

    public async Task<TokenResponse> CreateAsync(TokenNewRequest request)
    {
        var tokenId = Guid.NewGuid();
        var tokenResponse = await _jwtService.CreateTokenAsync(new AuthCreateTokenRequest()
        {
            Expires = DateTime.UtcNow.ToUniversalTime().AddSeconds(App.Setting.Security.TimeoutInSecond),
            Claims = new Dictionary<string, string>()
            {
                { "Id", tokenId.ToString() },
                { "UserId", request.User.Id.ToString() },
                { "Name", request.User.Name },
            }
        });

        var uaParser = Parser.GetDefault();
        var clientInfo = uaParser.Parse(request.UserAgent ?? "");

        var token = await _tokenRepository.InsertAsync(new Token()
        {
            Id = tokenId,
            UserId = request.User.Id,
            Os = clientInfo.OS.Family,
            Platform = clientInfo.Device.Family,
            Application = clientInfo.Browser.Family,
            State = TokenState.Valid
        });
        _memoryCache.Set(token.Id, token, TimeSpan.FromMinutes(1));
        
        return new TokenResponse()
        {
            Csrf = token.Id,
            Key = tokenResponse.Key,
        };
    }
}