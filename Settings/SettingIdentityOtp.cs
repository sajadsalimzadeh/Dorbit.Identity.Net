using Dorbit.Framework.Attributes;
using Dorbit.Framework.Contracts.Abstractions;

namespace Dorbit.Identity.Settings;

[AuthSetting("Setting-Identity-Otp")]
public class SettingIdentityOtp : ISettingDto
{
    public string TemplateCode { [AuthSetting] get; set; }

    public string GetKey()
    {
        return "dorbit.idenity.otp";
    }
}