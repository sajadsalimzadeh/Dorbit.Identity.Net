namespace Dorbit.Identity.Models.Tokens;

public class TokenNewRequest
{
    public Guid UserId { get; set; }
    public string UserAgent { get; set; }
}