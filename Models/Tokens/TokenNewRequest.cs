using Dorbit.Identity.Entities;

namespace Dorbit.Identity.Models.Tokens;

public class TokenNewRequest
{
    public User User { get; set; }
    public string UserAgent { get; set; }
}