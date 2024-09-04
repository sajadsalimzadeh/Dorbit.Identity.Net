namespace Dorbit.Identity;

public class IdentityAppSetting
{
    public AppSettingAdmin Admin { get; set; } = new();
    public AppSettingSecurity Security { get; set; } = new();
    
    public class AppSettingAdmin
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Cellphone { get; set; }
        public string Email { get; set; }
    }

    public class AppSettingSecurity
    {
    }
}
