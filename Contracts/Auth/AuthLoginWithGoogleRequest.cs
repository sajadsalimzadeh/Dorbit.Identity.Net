using Microsoft.AspNetCore.Mvc;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithGoogleRequest : AuthLoginRequest
{
    [FromQuery(Name = "code")] public string Code { get; set; }
    [FromQuery(Name = "token")] public string AccessToken { get; set; }
}