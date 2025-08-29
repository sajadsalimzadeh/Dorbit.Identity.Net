﻿using System;
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
using Microsoft.Extensions.Options;

namespace Dorbit.Identity.Controllers;

[Route("Identity/[controller]")]
public class AuthController(IdentityService identityService, IOptions<ConfigIdentitySecurity> configIdentitySecurity) : BaseController
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
            result.IsEmailVerificationRequired = !user.EmailVerificationTime.HasValue && configIdentitySecurity.Value.IsEmailVerificationRequired;
            result.IsCellphoneVerificationRequired = !user.CellphoneVerificationTime.HasValue && configIdentitySecurity.Value.IsCellphoneVerificationRequired;
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
    
    [HttpPost("LoginWithGoogle")]
    public async Task<QueryResult<AuthLoginResponse>> LoginWithGoogleAsync([FromBody] AuthLoginWithGoogleRequest request)
    {
        request.FillByRequest(Request);
        var loginResponse = await identityService.LoginWithGoogleAsync(request);
        return loginResponse.SetCookie(Response).ToQueryResult();
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

    [HttpDelete("Logout")]
    public Task<CommandResult> Logout()
    {
        Response.Cookies.Delete(nameof(TokenClaimTypes.CsrfToken));
        return Task.FromResult(new CommandResult(UserResolver.User is not null));
    }
}