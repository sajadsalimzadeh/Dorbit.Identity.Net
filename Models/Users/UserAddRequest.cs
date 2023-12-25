namespace Dorbit.Identity.Models.Users;

public class UserAddRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Cellphone { get; set; }
    public string Email { get; set; }
    public bool NeedResetPassword { get; set; }
}