using System.Collections.Generic;

namespace Dorbit.Identity.Configs;

public class ConfigGoogleOAuth
{
    public Dictionary<string, string> ClientIds { get; set; }
    public string ClientSecret { get; set; }
    public string RedirectUrl { get; set; }
    public string ReturnUrl { get; set; }
}