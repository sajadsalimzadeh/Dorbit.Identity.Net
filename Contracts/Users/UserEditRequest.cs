namespace Dorbit.Identity.Contracts.Users;

public class UserEditRequest : UserEditOwnRequest
{
    public string Username { get; set; }
    public bool NeedResetPassword { get; set; }
    public short MaxTokenCount { get; set; } = 1;
}