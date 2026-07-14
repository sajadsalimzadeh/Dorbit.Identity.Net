namespace Dorbit.Identity.Contracts.Users;

public class UserBaseEditOwnRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cellphone { get; set; }
    public string ThumbnailFilename { get; set; }
}