namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithGoogleRequest : AuthLoginRequest
{
    public string Token { get; set; }
}