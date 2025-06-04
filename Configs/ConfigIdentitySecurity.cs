using Dorbit.Framework.Contracts.Cryptograpy;

namespace Dorbit.Identity.Configs;

public class ConfigIdentitySecurity
{
    public string PasswordPattern { get; set; } = ".+";
    public int TimeoutInSecond { get; set; } = 300;
    public short OtpTimeoutInSec { get; set; } = 120;
    public int MaxActiveTokenCountPerUser { get; set; } = 10;
    public ProtectedProperty Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}