using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Contracts.Jwts;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services;
using Dorbit.Framework.Utils;
using Dorbit.Identity.Configs;
using Dorbit.Identity.Contracts;
using Dorbit.Identity.Contracts.Auth;
using Dorbit.Identity.Contracts.Tokens;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class TokenService(
    JwtService jwtService,
    IMemoryCache memoryCache,
    TokenRepository tokenRepository,
    IOptions<ConfigIdentitySecurity> configSecurityOptions)
{
    private readonly ConfigIdentitySecurity _configIdentitySecurity = configSecurityOptions.Value;

    public async Task<AuthLoginResponse> CreateAsync(TokenCreateRequest request)
    {
        var uaParser = UserAgentParser.GetDefault();
        var clientInfo = uaParser.Parse(request.UserAgent ?? "");
        var secret = _configIdentitySecurity.Secret.GetDecryptedValue();

        var maxActiveTokenCount = Math.Min(request.User.MaxTokenCount, _configIdentitySecurity.MaxActiveTokenCountPerUser);
        var activeTokens = await tokenRepository.Set().Where(x => x.UserId == request.User.Id && x.ExpireTime > DateTime.UtcNow).ToListAsync();
        foreach (var activeToken in activeTokens.OrderBy(x => x.ExpireTime).Take(activeTokens.Count - maxActiveTokenCount + 1))
        {
            memoryCache.Remove(activeToken.Id);
            activeToken.State = TokenState.Terminated;
            await tokenRepository.UpdateAsync(activeToken);
        }

        var isTwoFactorAuthenticated = false;
        var isNeddTwoFactorAuthentication = request.User.IsTwoFactorAuthenticationEnabled;
        if (request.AccessToken.IsNotNullOrEmpty())
        {
            if (jwtService.TryValidateToken(request.AccessToken, secret, out _, out var preClaimsPrincipal))
            {
                var needTwoFactorAuthenticationClaim =
                    preClaimsPrincipal.Claims.FirstOrDefault(x => x.Type == nameof(TokenClaimTypes.NeedTwoFactorAuthentication));

                if (needTwoFactorAuthenticationClaim is not null &&
                    bool.TryParse(needTwoFactorAuthenticationClaim.Value, out var isNeedTwoFactorAuthentication) && isNeedTwoFactorAuthentication)
                {
                    isNeddTwoFactorAuthentication = false;
                    isTwoFactorAuthenticated = true;
                }
            }
        }

        var tokenId = Guid.NewGuid();
        var token = await tokenRepository.InsertAsync(new Token()
        {
            Id = tokenId,
            UserId = request.User.Id,
            ExpireTime = DateTime.UtcNow.AddSeconds(_configIdentitySecurity.TimeoutInSecond),
            State = TokenState.Valid,
            TokenInfo = new TokenInfo()
            {
                Device = new ClientInfoVersion(clientInfo.Device.Family, clientInfo.Device.Brand, clientInfo.Device.Model),
                Os = new ClientInfoVersion(clientInfo.OS.Family, clientInfo.OS.Major, clientInfo.OS.Minor, clientInfo.OS.Patch),
                Browser = new ClientInfoVersion(clientInfo.UA.Family, clientInfo.UA.Major, clientInfo.UA.Minor, clientInfo.UA.Patch),
            }
        });

        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(nameof(TokenClaimTypes.Id), tokenId.ToString()));
        claims.AddClaim(new Claim(nameof(TokenClaimTypes.UserId), request.User.Id.ToString()));
        claims.AddClaim(new Claim(nameof(TokenClaimTypes.CsrfToken), request.CsrfToken));

        if (isNeddTwoFactorAuthentication) claims.AddClaim(new Claim(nameof(TokenClaimTypes.NeedTwoFactorAuthentication), "True"));
        if (isTwoFactorAuthenticated) claims.AddClaim(new Claim(nameof(TokenClaimTypes.TwoFactorAuthenticated), "True"));

        var accessToken = jwtService.CreateToken(new JwtCreateTokenRequest(secret, request.CsrfToken, token.ExpireTime)
        {
            Claims = claims
        });
        memoryCache.Set(token.Id, token, TimeSpan.FromMinutes(1));

        return new AuthLoginResponse()
        {
            AccessToken = accessToken,
            CsrfToken = request.CsrfToken,
            IsNeedAuthentication = isNeddTwoFactorAuthentication,
            TimeoutInSecond = _configIdentitySecurity.TimeoutInSecond
        };
    }
}