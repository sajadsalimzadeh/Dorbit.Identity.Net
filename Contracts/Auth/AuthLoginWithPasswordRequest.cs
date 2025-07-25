namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithPasswordRequest : AuthLoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}