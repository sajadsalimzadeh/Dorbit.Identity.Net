using Microsoft.Extensions.DependencyInjection;

namespace Dorbit.Identity;

public class AppSetting
{
    private static AppSetting _current;
    public static AppSetting Current => _current ??= Dorbit.App.ServiceProvider.GetService<AppSetting>();
    
    public AppSettingSecurity Security { get; set; }
}

public class AppSettingSecurity
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public short TimeoutInSecond { get; set; }
}