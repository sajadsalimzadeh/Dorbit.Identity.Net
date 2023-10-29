namespace Dorbit.Identity.Models.Tokens;

public class TokenNewRequest
{
    public Guid UserId { get; set; }
    public string Os { get; set; }
    public string Platform { get; set; }
    public string AppName { get; set; }
    public string AppVersion { get; set; }
    public List<string> Accesses { get; set; }
}