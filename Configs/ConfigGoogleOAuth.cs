namespace Dorbit.Identity.Configs;

public class ConfigGoogleOAuth
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string RedirectUrl { get; set; }
    public string ReturnUrl { get; set; }
}