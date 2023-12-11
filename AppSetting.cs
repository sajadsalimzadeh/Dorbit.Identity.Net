namespace Dorbit.Identity;

public class AppSetting
{
    public AppSettingAdmin Admin { get; set; } = new();
    public AppSettingSecurity Security { get; set; } = new();
    public AppSettingMessage Message { get; set; } = new();
}

public class AppSettingAdmin
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Cellphone { get; set; }
    public string Email { get; set; }
}

public class AppSettingSecurity
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public short TimeoutInSecond { get; set; }

    public short OtpTimeoutInSec { get; set; } = 120;
}

public class AppSettingMessage
{
    public string MeliPayamakOtpBodyId { get; set; }
}