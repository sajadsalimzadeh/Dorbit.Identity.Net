using Microsoft.AspNetCore.Mvc;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithGoogleRequest : AuthLoginRequest
{
    [FromQuery(Name = "code")]
    public string AuthenticationCode { get; set; }
}