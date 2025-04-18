namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithStaticPasswordRequest : AuthLoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}