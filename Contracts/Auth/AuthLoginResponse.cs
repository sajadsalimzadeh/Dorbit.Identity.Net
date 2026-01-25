using System;
using Dorbit.Identity.Contracts.Tokens;
using Microsoft.AspNetCore.Http;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginResponse
{
    public required string CsrfToken { get; set; }
    public required string AccessToken { get; set; }
    public int TimeoutInSecond { get; set; }
    public bool IsNeedAuthentication { get; set; }
    public DateTime ExpireTime { get; set; }

    public AuthLoginResponse SetCookie(HttpResponse response)
    {
        response.Cookies.Append(nameof(TokenClaimTypes.CsrfToken), CsrfToken, new CookieOptions()
        {
            Path = "/",
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddSeconds(TimeoutInSecond),
        });
        return this;
    }
}