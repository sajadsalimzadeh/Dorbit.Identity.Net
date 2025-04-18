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
public class ClientInfoVersion
{
    public string Family { get; set; }
    public string Major { get; set; }
    public string Minor { get; set; }
    public string Patch { get; set; }

    public ClientInfoVersion(string family = null, string major = null, string minor = null, string patch = null)
    {
        Family = family;
        Major = major;
        Minor = minor;
        Patch = patch;
    }
}