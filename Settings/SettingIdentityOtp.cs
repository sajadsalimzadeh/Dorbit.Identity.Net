using Dorbit.Framework.Contracts.Abstractions;

namespace Dorbit.Identity.Settings;

public class SettingIdentityOtp : ISettingDto
{
    public string TemplateCode { get; set; }
    
    public string GetKey()
    {
        return "dorbit.idenity.otp";
    }
}