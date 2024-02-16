namespace Dorbit.Identity.Contracts.Users;

public class UserSearchRequest
{
    public string Name { get; set; }
    public bool WaitForGroup { get; set; }
}