using Dorbit.Framework.Models.Users;

namespace Dorbit.Identity.Models.Users;

public class UserIdentityDto : UserDto
{
    public string Email { get; set; }
    public string Cellphone { get; set; }
    public string Username { get; set; }
    public bool IsTwoFactorAuthenticationEnable { get; set; }
    public bool NeedResetPassword { get; set; }
    public bool IsActive { get; set; }
}