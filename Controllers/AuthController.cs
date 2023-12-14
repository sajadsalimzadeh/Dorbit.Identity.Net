using Dorbit.Framework.Controllers;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Filters;
using Dorbit.Framework.Models;
using Dorbit.Identity.Models.Auth;
using Dorbit.Identity.Models.Users;
using Dorbit.Identity.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dorbit.Identity.Controllers;

public class AuthController : BaseController
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("[action]")]
    public async Task<QueryResult<AuthLoginResponse>> Login([FromBody] AuthLoginRequest request)
    {
        var loginResponse = await _authService.Login(request);
        if (loginResponse.Token is not null)
        {
            Response.Cookies.Append("Token", loginResponse.Token.Key, new CookieOptions()
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
        return Task.FromResult(new CommandResult(UserId is not null));
    }

    [HttpGet("[action]"), Auth]
    public Task<QueryResult<UserDto>> IsLogin()
    {
        return Task.FromResult(new QueryResult<UserDto>(UserResolver.User.MapTo<UserDto>()));
    }
}