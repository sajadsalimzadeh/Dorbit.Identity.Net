namespace Dorbit.Identity.Contracts.Users;

public class UserResetPasswordRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}