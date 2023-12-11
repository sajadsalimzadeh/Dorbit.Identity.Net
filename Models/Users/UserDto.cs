namespace Dorbit.Identity.Models.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cellphone { get; set; }
    public string Username { get; set; }
    public bool IsTwoFactorAuthenticationEnable { get; set; }
    public bool NeedResetPassword { get; set; }
    public bool IsActive { get; set; }
}