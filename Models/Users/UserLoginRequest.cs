using Dorbit.Identity.Enums;

namespace Dorbit.Identity.Models.Users;

public class UserLoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public UserLoginStrategy LoginStrategy { get; set; }
}