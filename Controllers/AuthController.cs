using System;
using System.Linq;
using System.Threading.Tasks;
using Dorbit.Framework.Contracts.Abstractions;
using Dorbit.Framework.Contracts.Identities;
using Dorbit.Framework.Contracts.Results;
using Dorbit.Framework.Controllers;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Filters;
using Dorbit.Identity.Configs;
using Dorbit.Identity.Contracts.Auth;
using Dorbit.Identity.Contracts.Tokens;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Dorbit.Identity.Controllers;

public class AuthController(
    IdentityService identityService,
    IOptions<ConfigIdentitySecurity> configSecurityOptions)
    : BaseController
{
    private readonly ConfigIdentitySecurity _configIdentitySecurity = configSecurityOptions.Value;

    private void HandleToken(AuthLoginResponse loginResponse)
    {
        if (loginResponse is not null)
        {
            Response.Cookies.Append(nameof(TokenClaimTypes.CsrfToken), loginResponse.CsrfToken, new CookieOptions()
            {
                Path = "/",
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddSeconds(_configIdentitySecurity.TimeoutInSecond),
            });
        }
    }

    [HttpGet, Auth]
    public QueryResult<IdentityDto> GetLoginInfo()
    {
        return identityService.Identity.ToQueryResult();
    }

    [HttpPost("LoginWithPassword"), Captcha]
    public async Task<QueryResult<AuthLoginResponse>> LoginWithPasswordAsync([FromBody] AuthLoginWithStaticPasswordRequest withStaticPasswordRequest)
    {
        if (HttpContext.Request.Headers.TryGetValue("User-Agent", out var userAgent))
        {
            withStaticPasswordRequest.UserAgent = userAgent;
        }

        var loginResponse = await identityService.LoginWithStaticPasswordAsync(withStaticPasswordRequest);
        HandleToken(loginResponse);
        return loginResponse.ToQueryResult();
    }

    [HttpPost("LoginWithOtp")]
    public async Task<QueryResult<AuthLoginResponse>> LoginWithOtpAsync([FromBody] AuthLoginWithOtpRequest request)
    {
        var loginResponse = await identityService.LoginWithOtpAsync(request);
        HandleToken(loginResponse);
        return loginResponse.ToQueryResult();
    }

    [HttpPost("Register")]
    public async Task<QueryResult<AuthLoginResponse>> Register([FromBody] AuthRegisterRequest request)
    {
        var loginResponse = await identityService.RegisterAsync(request);
        HandleToken(loginResponse);
        return loginResponse.ToQueryResult();
    }

    [HttpDelete("Logout")]
    public Task<CommandResult> Logout()
    {
        Response.Cookies.Delete(nameof(TokenClaimTypes.CsrfToken));
        return Task.FromResult(new CommandResult(UserResolver.User is not null));
    }
}