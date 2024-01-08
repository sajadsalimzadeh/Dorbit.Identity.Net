using Dorbit.Identity.Enums;

namespace Dorbit.Identity.Models.Auth;

public class AuthLoginRequest
{
    public string Username { get; set; }
    public string UserAgent { get; set; }
    
    public string Value { get; set; }
    public UserLoginStrategy LoginStrategy { get; set; }
}