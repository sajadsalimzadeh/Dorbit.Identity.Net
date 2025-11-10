using Microsoft.AspNetCore.Mvc;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithAppleRequest : AuthLoginRequest
{
    public string State { get; set; }
    [FromForm(Name = "code")]
    public string AuthenticationCode { get; set; }
    [FromForm(Name = "id_token")]
    public string IdToken { get; set; }
}