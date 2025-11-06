namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithGoogleRequest : AuthLoginRequest
{
    public string AuthorizationCode { get; set; }
    public string RedirectUrl { get; set; }
}