using System;
using System.Threading.Tasks;
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

public class AuthController : BaseController
{
    private readonly AuthService _authService;
    private readonly UserRepository _userRepository;
    private readonly PrivilegeService _privilegeService;
    private readonly ConfigIdentitySecurity _configIdentitySecurity;

    public AuthController(
        AuthService authService, 
        UserRepository userRepository, 
        PrivilegeService privilegeService,
        IOptions<ConfigIdentitySecurity> configSecurityOptions)
    {
        _authService = authService;
        _userRepository = userRepository;
        _privilegeService = privilegeService;
        _configIdentitySecurity = configSecurityOptions.Value;
    }

    private void HandleToken(TokenResponse tokenResponse)
    {
        if (tokenResponse is not null)
        {
            Response.Cookies.Append("CSRF", tokenResponse.Csrf.ToString(), new CookieOptions()
            {
                Expires = DateTime.UtcNow.AddSeconds(_configIdentitySecurity.TimeoutInSecond),
                HttpOnly = true,
                Secure = false,
                Path = "/"
            });
        }
    }

    [HttpPost("Login")]
    public async Task<QueryResult<AuthLoginResponse>> Login([FromBody] AuthLoginRequest request)
    {
        var loginResponse = await _authService.LoginAsync(request);
        HandleToken(loginResponse.Token);
        return loginResponse.ToQueryResult();
    }

    [HttpPost("LoginWithCode")]
    public async Task<QueryResult<AuthLoginResponse>> LoginWithCode([FromBody] AuthLoginWithCodeRequest request)
    {
        var loginResponse = await _authService.LoginWithCodeAsync(request);
        HandleToken(loginResponse.Token);
        return loginResponse.ToQueryResult();
    }

    [HttpPost("Register")]
    public async Task<QueryResult<AuthLoginResponse>> Register([FromBody] AuthRegisterRequest request)
    {
        var loginResponse = await _authService.RegisterAsync(request);
        HandleToken(loginResponse.Token);
        return loginResponse.ToQueryResult();
    }

    [HttpDelete("Logout")]
    public Task<CommandResult> Logout()
    {
        Response.Cookies.Append("CSRF", "", new CookieOptions()
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
        var user = await _userRepository.GetByIdAsync(GetUserId<Guid>());
        var dto = user.MapTo<UserDto>();
        dto.Accesses = await _privilegeService.GetAllByUserIdAsync(user.Id);
        return dto.ToQueryResult();
    }
}