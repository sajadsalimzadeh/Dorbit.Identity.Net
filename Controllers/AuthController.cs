﻿using System;
using System.Threading.Tasks;
using Dorbit.Framework.Contracts;
using Dorbit.Framework.Contracts.Users;
using Dorbit.Framework.Controllers;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Filters;
using Dorbit.Identity.Contracts.Auth;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dorbit.Identity.Controllers;

public class AuthController : BaseController
{
    private readonly AuthService _authService;
    private readonly UserRepository _userRepository;
    private readonly PrivilegeService _privilegeService;

    public AuthController(AuthService authService, UserRepository userRepository, PrivilegeService privilegeService)
    {
        _authService = authService;
        _userRepository = userRepository;
        _privilegeService = privilegeService;
    }

    [HttpPost("[action]")]
    public async Task<QueryResult<AuthLoginResponse>> Login([FromBody] AuthLoginRequest request)
    {
        var loginResponse = await _authService.Login(request);
        if (loginResponse.Token is not null)
        {
            Response.Cookies.Append("CSRF", loginResponse.Token.Csrf.ToString(), new CookieOptions()
            {
                Expires = DateTime.UtcNow.AddMinutes(30),
                HttpOnly = true,
                Secure = false,
                Path = "/"
            });
        }

        return loginResponse.ToQueryResult();
    }

    [HttpDelete("[action]")]
    public Task<CommandResult> Logout()
    {
        Response.Cookies.Append("Token", "", new CookieOptions()
        {
            Expires = DateTime.UtcNow.AddMinutes(-30),
            HttpOnly = true,
            Secure = false,
            Path = "/"
        });
        return Task.FromResult(new CommandResult(UserResolver.User is not null));
    }

    [HttpGet("IsLogin"), Auth]
    public async Task<QueryResult<UserDto>> IsLogin()
    {
        var user = await _userRepository.GetByIdAsync(UserId);
        var dto = user.MapTo<UserDto>();
        dto.Accesses = await _privilegeService.GetAllByUserIdAsync(user.Id);
        return dto.ToQueryResult();
    }
}