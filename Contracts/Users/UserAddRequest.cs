using System;

namespace Dorbit.Identity.Contracts.Users;

public class UserAddRequest
{
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Cellphone { get; set; }
    public string Email { get; set; }
    public string AuthenticatorKey { get; set; }
    public bool NeedResetPassword { get; set; }

    public UserValidateTypes ValidateTypes { get; set; }
}