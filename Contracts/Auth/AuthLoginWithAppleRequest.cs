namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithAppleRequest : AuthLoginRequest
{
    public string Code { get; set; }
    public string IdToken { get; set; }
}