using Microsoft.AspNetCore.Mvc;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithGoogleRequest : AuthLoginRequest
{
    [FromQuery(Name = "code")] public string Code { get; set; }
    [FromQuery(Name = "verifier")] public string Verifier { get; set; }
    [FromQuery(Name = "platform")] public string Platform { get; set; } = "web";
}