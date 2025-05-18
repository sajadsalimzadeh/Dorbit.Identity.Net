using System.ComponentModel.DataAnnotations.Schema;

namespace Dorbit.Identity.Contracts.Tokens;

[NotMapped]
public class TokenInfo
{
    public ClientInfoVersion Device { get; set; }
    public ClientInfoVersion Os { get; set; }
    public ClientInfoVersion Browser { get; set; }
}

[NotMapped]
public class ClientInfoVersion(string family = null, string major = null, string minor = null, string patch = null)
{
    public string Family { get; set; } = family;
    public string Major { get; set; } = major;
    public string Minor { get; set; } = minor;
    public string Patch { get; set; } = patch;
}