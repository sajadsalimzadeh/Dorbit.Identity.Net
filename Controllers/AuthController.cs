using Dorbit.Framework.Controllers;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Filters;
using Dorbit.Framework.Models;
using Dorbit.Identity.Models.Auth;
using Dorbit.Identity.Models.Users;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dorbit.Identity.Controllers;

public class AuthController : BaseController
{
    private readonly AuthService _authService;
    private readonly UserRepository _userRepository;

    public AuthController(AuthService authService, UserRepository userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
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
        return Task.FromResult(new CommandResult(true));
    }

    [HttpGet("[action]"), Auth]
    public async Task<QueryResult<UserIdentityDto>> IsLogin()
    {
        var user = await _userRepository.GetByIdAsync(UserId);
        return user.MapTo<UserIdentityDto>().ToQueryResult();
    }
}