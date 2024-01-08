namespace Dorbit.Identity.Models.Users;

public class UserChangePasswordRequest
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string RenewPassword { get; set; }
}