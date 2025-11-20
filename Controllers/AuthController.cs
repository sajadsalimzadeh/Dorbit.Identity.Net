using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Dorbit.Identity.Controllers;

[Route("Identity/[controller]")]
public class AuthController(
    IdentityService identityService,
    IOptions<ConfigIdentitySecurity> configIdentitySecurity,
    IOptions<ConfigGoogleOAuth> configGoogleOAuthOptions,
    IOptions<ConfigAppleOAuth> configAppleOAuthOptions
) : BaseController
{
    [HttpGet, Auth]
    public QueryResult<AuthIdentityDto> GetLoginInfo()
    {
        var identity = identityService.Identity;
        var result = new AuthIdentityDto()
        {
            IsFullAccess = identity.IsFullAccess,
            Accessibility = identity.Accessibility,
            User = identity.User.MapTo(new UserDto()),
        };
        if (identity.User is UserDto user)
        {
            result.IsEmailVerified = user.EmailVerificationTime.HasValue;
            result.IsCellphoneVerified = user.CellphoneVerificationTime.HasValue;
        }

        return result.ToQueryResult();
    }

    [HttpPost("LoginWithPassword"), Captcha]
    public async Task<QueryResult<AuthLoginResponse>> LoginWithPasswordAsync([FromBody] AuthLoginWithPasswordRequest request)
    {
        request.FillByRequest(Request);
        var loginResponse = await identityService.LoginWithPasswordAsync(request);
        return loginResponse.SetCookie(Response).ToQueryResult();
    }

    [HttpGet("LoginWithGoogle")]
    public async Task<RedirectResult> LoginWithGoogleAsync([FromQuery] AuthLoginWithGoogleRequest request)
    {
        try
        {
            request.FillByRequest(Request);
            var loginResponse = await identityService.LoginWithGoogleAsync(request);
            loginResponse.SetCookie(Response);
            return Redirect($"{configGoogleOAuthOptions.Value.ReturnUrl}?access_token={Uri.EscapeDataString(loginResponse.AccessToken)}&timeoutInSecond={loginResponse.TimeoutInSecond}");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.Message);
            return Redirect($"{configGoogleOAuthOptions.Value.ReturnUrl}");
        }
    }

    [HttpPost("LoginWithApple")]
    public async Task<RedirectResult> LoginWithAppleAsync([FromForm] AuthLoginWithAppleRequest request)
    {
        try
        {
            request.FillByRequest(Request);
            var loginResponse = await identityService.LoginWithAppleAsync(request);
            loginResponse.SetCookie(Response);
            return Redirect($"{configAppleOAuthOptions.Value.ReturnUrl}?access_token={Uri.EscapeDataString(loginResponse.AccessToken)}&timeoutInSecond={loginResponse.TimeoutInSecond}");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.Message);
            return Redirect($"{configGoogleOAuthOptions.Value.ReturnUrl}");
        }
    }

    [HttpPost("LoginWithOtp")]
    public async Task<QueryResult<AuthLoginResponse>> LoginWithOtpAsync([FromBody] AuthLoginWithOtpRequest request)
    {
        request.FillByRequest(Request);
        var loginResponse = await identityService.LoginWithOtpAsync(request);
        return loginResponse.SetCookie(Response).ToQueryResult();
    }

    [HttpPost("Register")]
    public async Task<QueryResult<AuthLoginResponse>> Register([FromBody] AuthRegisterRequest request)
    {
        var loginResponse = await identityService.RegisterAsync(request);
        return loginResponse.SetCookie(Response).ToQueryResult();
    }

    [HttpPost("ForgetPassword")]
    public async Task<QueryResult<AuthLoginResponse>> Register([FromBody] AuthForgetPasswordRequest request)
    {
        var loginResponse = await identityService.ForgetPasswordAsync(request);
        return loginResponse.SetCookie(Response).ToQueryResult();
    }

    [HttpDelete("Logout")]
    public Task<CommandResult> Logout()
    {
        Response.Cookies.Delete(nameof(TokenClaimTypes.CsrfToken));
        return Task.FromResult(new CommandResult(UserResolver.User is not null));
    }
}