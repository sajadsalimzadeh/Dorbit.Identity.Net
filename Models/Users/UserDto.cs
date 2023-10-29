namespace Dorbit.Identity.Models.Users;

public class UserDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public bool IsActive { get; set; }
}