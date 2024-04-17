using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Contracts.Jwts;
using Dorbit.Framework.Services;
using Dorbit.Identity.Contracts;
using Dorbit.Identity.Contracts.Tokens;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        var uaParser = await Parser.GetDefaultAsync();
        var clientInfo = await uaParser.ParseAsync(request.UserAgent ?? "");

        var maxActiveTokenCount = Math.Min(request.User.ActiveTokenCount, AppIdentity.Setting.Security.MaxActiveTokenCountPerUser);
        var activeTokens = await _tokenRepository.Set().Where(x => x.UserId == request.User.Id && x.ExpireTime > DateTime.UtcNow).ToListAsync();
        foreach (var activeToken in activeTokens.OrderBy(x => x.ExpireTime).Take(activeTokens.Count - maxActiveTokenCount + 1))
        {
            _memoryCache.Remove(activeToken.Id);
            activeToken.State = TokenState.Terminated;
            await _tokenRepository.UpdateAsync(activeToken);
        }

        var tokenId = Guid.NewGuid();
        var token = await _tokenRepository.InsertAsync(new Token()
        {
            Id = tokenId,
            UserId = request.User.Id,
            Os = clientInfo.OS.Family,
            Platform = clientInfo.Device.Family,
            Application = clientInfo.Browser.Family,
            ExpireTime = DateTime.UtcNow.AddSeconds(AppIdentity.Setting.Security.TimeoutInSecond),
            State = TokenState.Valid
        });

        var tokenResponse = await _jwtService.CreateTokenAsync(new JwtCreateTokenRequest()
        {
            Expires = token.ExpireTime,
            Claims = new Dictionary<string, string>()
            {
                { "Id", tokenId.ToString() },
                { "UserId", request.User.Id.ToString() },
                { "Name", request.User.Name },
            }
        });
        _memoryCache.Set(token.Id, token, TimeSpan.FromMinutes(1));

        return new TokenResponse()
        {
            Csrf = token.Id,
            Key = tokenResponse.Key,
        };
    }
}